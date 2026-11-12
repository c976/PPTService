using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cvte.Documents;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Inks
{
	internal class MaskEraserHandler : EraserHandler
	{
		private readonly Slide _currentSlide;

		private readonly List<IOperation> _operations = new List<IOperation>();

		private EraserMask _mask;

		public MaskEraserHandler(Slide slide)
		{
			_currentSlide = slide;
		}

		public override void OnErasingSectionStarted(Point point)
		{
			_mask = new EraserMask(_currentSlide);
		}

		public override void OnErasingSectionCompleted()
		{
			ElementOperation elementOperation = _mask.CreateUndoRedo();
			if (elementOperation != null)
			{
				_operations.Add(elementOperation);
			}
			IList<Element> erasedElementList = _mask.GetErasedElementList();
			foreach (IEraseSensitive item3 in erasedElementList.OfType<IEraseSensitive>())
			{
				item3.OnErasingCompleted();
			}
			List<Element> list = new List<Element>();
			List<Element> list2 = new List<Element>();
			foreach (ISplitErasableInk item4 in erasedElementList.OfType<ISplitErasableInk>())
			{
				List<Element> list3 = item4.CreateNewInkOnErased().ToList();
				if (list3.Any())
				{
					list2.AddRange(list3);
				}
				list.Add((Element)item4);
			}
			Slide currentSlide = _currentSlide;
			if (list2.Any())
			{
				currentSlide.Add(list2);
				AddElementsOperation item = new AddElementsOperation(currentSlide, list2);
				_operations.Add(item);
			}
			list.AddRange(erasedElementList.Except(list).Where((Element temp) =>
			{
				Rect externalBounds = temp.Outline.GetExternalBounds();
				return externalBounds.Width < 0.01 && externalBounds.Height < 0.01;
			}));
			if (list.Any())
			{
				RemoveElementsOperation item2 = new RemoveElementsOperation(currentSlide, list);
				currentSlide.Remove(list);
				_operations.Add(item2);
			}
		}

		public override void EraseCore(IList<Point> points)
		{
			foreach (Point point in points)
			{
				_mask.MoveTexture(point, EraserSize);
			}
		}

		public override IOperation OnErasingCompleted()
		{
			if (_operations.Any())
			{
				return new ElementCombinationOperation(_currentSlide, _operations);
			}
			return null;
		}
	}
}
