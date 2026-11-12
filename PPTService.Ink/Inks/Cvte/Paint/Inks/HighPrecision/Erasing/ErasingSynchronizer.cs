using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Cvte.Documents;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public class ErasingSynchronizer
	{
		private Dictionary<Element, ValueTuple<Geometry, Transform>> _erasableElements;

		private List<Ink> _erasableInks;

		private Dictionary<Element, Geometry> _erasedElements;

		private HashSet<Ink> _erasedInks;

		private bool _finishEraseElements;

		private bool _finishEraseInks;

		public Slide CurrentSlide { get; }

		public ErasingSynchronizer(Slide slide)
		{
			CurrentSlide = slide;
		}

		public void Reset()
		{
			_finishEraseElements = false;
			_finishEraseInks = false;
			_erasableElements = CurrentSlide.Elements.Where((Element e) => e.IsErasable || e.HasMask).ToDictionary((Element e) => e, (Element e) =>
			{
				Rect rect = e.Outline.GetExternalBounds().Expand(1.0, 1.0, 1.0, 1.0);
				Rect bounds = e.Bounds;
				bounds.Transform(e.Transform);
				Point point = bounds.Center();
				RectangleGeometry item = new RectangleGeometry(new Rect(new Point(rect.Left - bounds.Left, rect.Top - bounds.Top), rect.Size));
				TransformGroup transformGroup = new TransformGroup();
				Matrix transform = e.Transform;
				transform.Invert();
				transformGroup.Children.Add(new MatrixTransform(transform));
				transformGroup.Children.Add(new RotateTransform(0.0 - e.Rotation, point.X, point.Y));
				transformGroup.Children.Add(new TranslateTransform(0.0 - e.Location.X, 0.0 - e.Location.Y));
				return new ValueTuple<Geometry, Transform>(item, transformGroup);
			});
			_erasedElements = new Dictionary<Element, Geometry>();
			_erasableInks = CurrentSlide.Elements.OfType<Ink>().ToList();
			_erasedInks = new HashSet<Ink>();
		}

		public IReadOnlyDictionary<Element, ValueTuple<Geometry, Transform>> GetErasableElements()
		{
			return new ReadOnlyDictionary<Element, ValueTuple<Geometry, Transform>>(_erasableElements);
		}

		public IReadOnlyList<Ink> GetErasableInks()
		{
			return _erasableInks.AsReadOnly();
		}

		public void StartEraseElement(Element element)
		{
			if (!_finishEraseElements && !_erasedElements.ContainsKey(element))
			{
				_erasedElements.Add(element, element.ClipGeometry);
				(element as IEraseSensitive)?.OnErasingStarted();
			}
		}

		public IList<IOperation> FinishEraseElements()
		{
			if (_finishEraseElements)
			{
				return new IOperation[0];
			}
			_finishEraseElements = true;
			List<IOperation> list = new List<IOperation>();
			list.Add(new ErasedElementOperation(CurrentSlide, _erasedElements.Select((KeyValuePair<Element, Geometry> pair) => new EraseOperationInfo(pair.Key)
			{
				OldGeometry = pair.Value,
				NewGeometry = pair.Key.ClipGeometry
			}).ToArray()));
			foreach (IEraseSensitive item3 in _erasedElements.Keys.OfType<IEraseSensitive>())
			{
				item3.OnErasingCompleted();
			}
			List<Element> list2 = new List<Element>();
			List<Element> list3 = new List<Element>();
			foreach (ISplitErasableInk item4 in _erasedElements.Keys.OfType<ISplitErasableInk>())
			{
				List<Element> list4 = item4.CreateNewInkOnErased().ToList();
				if (list4.Any())
				{
					list3.AddRange(list4);
				}
				list2.Add((Element)item4);
			}
			if (list3.Any())
			{
				CurrentSlide.Add(list3);
				AddElementsOperation item = new AddElementsOperation(CurrentSlide, list3);
				list.Add(item);
			}
			list2.AddRange(_erasedElements.Keys.Except(list2).Where((Element temp) =>
			{
				Rect externalBounds = temp.Outline.GetExternalBounds();
				return externalBounds.Width < 0.01 && externalBounds.Height < 0.01;
			}));
			if (list2.Any())
			{
				RemoveElementsOperation item2 = new RemoveElementsOperation(CurrentSlide, list2);
				CurrentSlide.Remove(list2);
				list.Add(item2);
			}
			return list;
		}

		public void StartEraseInk(Ink ink)
		{
			if (!_finishEraseInks && !_erasedInks.Contains(ink))
			{
				_erasedInks.Add(ink);
				((IEraseSensitive)ink).OnErasingStarted();
			}
		}

		public IList<IOperation> FinishEraseInks()
		{
			if (_finishEraseInks)
			{
				return new IOperation[0];
			}
			List<IOperation> list = new List<IOperation>();
			List<Ink> list2 = new List<Ink>();
			foreach (Ink erasedInk in _erasedInks)
			{
				IEnumerable<Element> enumerable = ((ISplitErasableInk)erasedInk).CreateNewInkOnErased();
				CurrentSlide.Add(enumerable);
				list2.AddRange(enumerable.Cast<Ink>());
				((IEraseSensitive)erasedInk).OnErasingCompleted();
				if (CurrentSlide.Contains(erasedInk))
				{
					CurrentSlide.Remove(erasedInk);
				}
			}
			if (_erasableInks.Any())
			{
				list.Add(new ElementCombinationOperation(CurrentSlide, new AddElementsOperation(CurrentSlide, list2), new RemoveElementsOperation(CurrentSlide, _erasableInks)));
			}
			return list;
		}
	}
}
