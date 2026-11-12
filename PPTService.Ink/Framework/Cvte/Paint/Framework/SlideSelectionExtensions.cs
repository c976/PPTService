using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.SelectionBoxs;

namespace Cvte.Paint.Framework
{
	public static class SlideSelectionExtensions
	{
		private static readonly DependencyProperty SelectionBoxLayerProperty = DependencyProperty.RegisterAttached("SelectionBoxLayer", typeof(SelectionBoxLayer), typeof(SlideSelectionExtensions), new PropertyMetadata((object)null));

		private static readonly DependencyProperty EditableSelectedElements = DependencyProperty.RegisterAttached("EditableSelectedElements", typeof(LevelSortedElementCollection), typeof(SlideSelectionExtensions), new PropertyMetadata((object)null));

		private static readonly DependencyProperty SelectedElementCollectionProperty = DependencyProperty.RegisterAttached("SelectedElementCollection", typeof(ReadOnlyCollection<Element>), typeof(SlideSelectionExtensions), new PropertyMetadata(null));

		public static SelectionBoxLayer GetSelectionBoxLayer(this Slide slide)
		{
			SelectionBoxLayer selectionBoxLayer = (SelectionBoxLayer)slide.GetValue(SelectionBoxLayerProperty);
			if (selectionBoxLayer == null)
			{
				selectionBoxLayer = new SelectionBoxLayer();
				slide.SetValue(SelectionBoxLayerProperty, selectionBoxLayer);
			}
			return selectionBoxLayer;
		}

		public static IEnumerable<ISelectionOperator> GetSelectionOperators(this Slide slide)
		{
			SelectionBoxLayer selectionBoxLayer = (SelectionBoxLayer)slide.GetValue(SelectionBoxLayerProperty);
			if (selectionBoxLayer == null)
			{
				return Enumerable.Empty<ISelectionOperator>();
			}
			return selectionBoxLayer.Children.OfType<ISelectionOperator>();
		}

		[CanBeNull]
		public static SelectionBoxLayer GetAttachedSelectionBoxLayer(this Slide slide)
		{
			SelectionBoxLayer selectionBoxLayer = slide.GetSelectionBoxLayer();
			if (!selectionBoxLayer.IsAttached && !selectionBoxLayer.TryAttach(slide))
			{
				return null;
			}
			return selectionBoxLayer;
		}

		public static void DetachSelectionBoxLayerWhenEmpty(this Slide slide)
		{
			SelectionBoxLayer selectionBoxLayer = (SelectionBoxLayer)slide.GetValue(SelectionBoxLayerProperty);
			if (selectionBoxLayer != null && !selectionBoxLayer.Children.Any())
			{
				if (selectionBoxLayer.IsAttached)
				{
					selectionBoxLayer.Detach();
				}
				slide.ClearValue(SelectionBoxLayerProperty);
			}
		}

		public static bool GetIsTransforming(this Element element)
		{
			return Slide.GetSlide(element)?.GetSelectionOperators().Where((ISelectionOperator x) => x.SelectedElements.Contains(element)).Any((ISelectionOperator x) => x.IsTransforming) ?? false;
		}

		public static bool GetIsSelected(this Element element)
		{
			return Slide.GetSlide(element)?.GetSelectedElements().Contains(element) ?? false;
		}

		[CanBeNull]
		internal static ISelectionOperator GetContainingOperator([NotNull] this Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			Slide slide = Slide.GetSlide(element);
			if (slide == null)
			{
				throw new InvalidOperationException("不在 Slide 上的元素无法获取选择框。");
			}
			return slide.GetSelectionOperators().FirstOrDefault((ISelectionOperator x) => x.SelectedElements.Contains(element));
		}

		[NotNull]
		internal static LevelSortedElementCollection GetEditableSelectedElements(this Slide slide)
		{
			LevelSortedElementCollection levelSortedElementCollection = (LevelSortedElementCollection)slide.GetValue(EditableSelectedElements);
			if (levelSortedElementCollection == null)
			{
				levelSortedElementCollection = InitSlideSelectedElementsProperty(slide).Item1;
			}
			return levelSortedElementCollection;
		}

		[Obsolete("此方法已过时，请使用 GetSelectedElements 代替")]
		internal static void SelectedElements(this Slide slide)
		{
			throw new NotSupportedException("此方法已过时，请使用 GetSelectedElements 代替");
		}

		[NotNull]
		public static ReadOnlyCollection<Element> GetSelectedElements(this Slide slide)
		{
			ReadOnlyCollection<Element> readOnlyCollection = (ReadOnlyCollection<Element>)slide.GetValue(SelectedElementCollectionProperty);
			if (readOnlyCollection == null)
			{
				readOnlyCollection = InitSlideSelectedElementsProperty(slide).Item2;
			}
			return readOnlyCollection;
		}

		private static ValueTuple<LevelSortedElementCollection, ReadOnlyCollection<Element>> InitSlideSelectedElementsProperty(Slide slide)
		{
			LevelSortedElementCollection levelSortedElementCollection = new LevelSortedElementCollection(slide);
			ReadOnlyCollection<Element> readOnlyCollection = new ReadOnlyCollection<Element>(levelSortedElementCollection);
			slide.SetValue(EditableSelectedElements, levelSortedElementCollection);
			slide.SetValue(SelectedElementCollectionProperty, readOnlyCollection);
			return new ValueTuple<LevelSortedElementCollection, ReadOnlyCollection<Element>>(levelSortedElementCollection, readOnlyCollection);
		}

		public static ReadOnlyCollection<Element> GetOriginSelectedElements(this Slide slide)
		{
			return ((LevelSortedElementCollection)slide.GetValue(EditableSelectedElements)).Origin;
		}

		public static void Select(this Slide slide, IEnumerable<Element> elements)
		{
			slide.ClearAndSelect((elements as Element[]) ?? elements.ToArray());
		}

		public static void Unselect(this Slide slide)
		{
			slide.ClearAndSelect();
		}

		public static bool TrySelect(this Slide slide, IEnumerable<Element> elements)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			if (slide.IsLoaded)
			{
				slide.Select(elements);
				return true;
			}
			return false;
		}

		public static bool TryUnselect(this Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (slide.IsLoaded)
			{
				slide.Unselect();
				return true;
			}
			return false;
		}

		public static bool TryReselect(this Slide slide, IEnumerable<Element> elements)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			if (slide.IsLoaded)
			{
				slide.Reselect((elements as IList<Element>) ?? elements.ToList());
				return true;
			}
			return false;
		}
	}
}
