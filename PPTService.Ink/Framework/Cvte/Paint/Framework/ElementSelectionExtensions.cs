using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Linq;
using Cvte.Paint.Framework.Elements;
using Cvte.Paint.Framework.SelectionBoxs;
using Cvte.Paint.Performance;
using Cvte.Windows;

namespace Cvte.Paint.Framework
{
	public static class ElementSelectionExtensions
	{
		public static readonly DependencyProperty CanSelectProperty = DependencyProperty.RegisterAttached("CanSelect", typeof(bool), typeof(ElementHitTester), new PropertyMetadata(true));

		private static readonly DependencyProperty SelectionBoxGeneratorProperty = DependencyProperty.RegisterAttached("SelectionBoxGenerator", typeof(ISelectionBoxGenerator), typeof(ElementSelectionExtensions), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static bool GetCanSelect(Element element)
		{
			return (bool)element.GetValue(CanSelectProperty);
		}

		public static void SetSelectionBoxGenerator(this Board board, ISelectionBoxGenerator generator)
		{
			IBoardSensitiveSelectionBoxGenerator boardSensitiveSelectionBoxGenerator = generator as IBoardSensitiveSelectionBoxGenerator;
			if (boardSensitiveSelectionBoxGenerator != null)
			{
				board.SetSelectionBoxGenerator(boardSensitiveSelectionBoxGenerator);
			}
			else
			{
				board.SetValue(SelectionBoxGeneratorProperty, generator);
			}
		}

		public static void SetSelectionBoxGenerator(this Board board, IBoardSensitiveSelectionBoxGenerator generator)
		{
			board.SetValue(SelectionBoxGeneratorProperty, generator);
			generator.Board = board;
		}

		public static ISelectionBoxGenerator GetSelectionBoxGenerator(this Board board)
		{
			ISelectionBoxGenerator selectionBoxGenerator = (ISelectionBoxGenerator)board.GetValue(SelectionBoxGeneratorProperty);
			if (selectionBoxGenerator == null)
			{
				selectionBoxGenerator = GetSelectionBoxGeneratorFromInterfaceCreator();
				board.SetSelectionBoxGenerator(selectionBoxGenerator);
			}
			return selectionBoxGenerator;
		}

		public static ISelectionBoxGenerator GetSelectionBoxGenerator(this Slide slide)
		{
			ISelectionBoxGenerator selectionBoxGenerator = (ISelectionBoxGenerator)slide.GetValue(SelectionBoxGeneratorProperty);
			if (selectionBoxGenerator == null)
			{
				Board board = slide.VisualAncestor<Board>();
				if (board != null)
				{
					return board.GetSelectionBoxGenerator();
				}
			}
			return selectionBoxGenerator;
		}

		public static ISelectionBoxGenerator GetSelectionBoxGenerator(this Element element)
		{
			ISelectionBoxGenerator selectionBoxGenerator = (ISelectionBoxGenerator)element.GetValue(SelectionBoxGeneratorProperty);
			if (selectionBoxGenerator == null)
			{
				Board board = element.VisualAncestor<Board>();
				if (board != null)
				{
					return board.GetSelectionBoxGenerator();
				}
			}
			return selectionBoxGenerator;
		}

		private static ISelectionBoxGenerator GetSelectionBoxGeneratorFromInterfaceCreator()
		{
			ISelectionBoxGenerator interfaceInstance = InterfaceCreator.GetInterfaceInstance<ISelectionBoxGenerator>(new object[0]);
			if (interfaceInstance == null && DebuggingProperties.IsDebug)
			{
				throw new InvalidOperationException("没有从 InterfaceCreator 里面找到 ISelectionBoxGenerator 的创建定义，请确定已初始化 Paint 模块，如调用 Cvte.Paint.Features.Initializer 等进行初始化。仅调试下抛出，请找德熙");
			}
			return interfaceInstance;
		}

		[Obsolete("选择框已经不需要Operator，请选择使用选择框的基类SelectionBoxBase和SingleSelectionBoxBase代替")]
		public static ISelectionOperator GetSelectionOperator(this ISelectionBox box)
		{
			return (ISelectionOperator)box;
		}

		public static void Select(this Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			Stack<KeyValuePair<IGroupElement, Element>> stack = new Stack<KeyValuePair<IGroupElement, Element>>();
			FrameworkElement frameworkElement = element;
			while (true)
			{
				FrameworkElement frameworkElement2 = frameworkElement.Parent as FrameworkElement;
				if (frameworkElement2 == null || frameworkElement2 is ElementCanvas)
				{
					break;
				}
				if (frameworkElement is Element && !(frameworkElement2 is IGroupElement))
				{
					frameworkElement2 = frameworkElement2.Parent as FrameworkElement;
				}
				Element element2 = frameworkElement as Element;
				IGroupElement groupElement = frameworkElement2 as IGroupElement;
				if (element2 != null && groupElement != null)
				{
					stack.Push(new KeyValuePair<IGroupElement, Element>(groupElement, element2));
				}
				frameworkElement = frameworkElement2;
			}
			Element element3 = (Element)frameworkElement;
			Slide.GetSlide(element3)?.ClearAndSelect(element3);
			while (stack.Any())
			{
				KeyValuePair<IGroupElement, Element> keyValuePair = stack.Pop();
				keyValuePair.Key.InnerSelect(keyValuePair.Value);
			}
		}

		public static void AppendSelect([NotNull] this Slide slide, [NotNull] params Element[] elementsToSelect)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elementsToSelect == null)
			{
				throw new ArgumentNullException("elementsToSelect");
			}
			elementsToSelect = FilterHitTest(elementsToSelect).TopGroupElements();
			elementsToSelect = elementsToSelect.Except(slide.GetSelectedElements()).ToArray();
			if (!elementsToSelect.Any())
			{
				return;
			}
			ISelectionBoxGenerator selectionBoxGenerator = elementsToSelect.First().GetSelectionBoxGenerator();
			if (selectionBoxGenerator != null)
			{
				if (!slide.IsLoaded)
				{
					throw new InvalidOperationException("不能选择一个并没有显示的 Slide 上的元素。");
				}
				SelectAndAddCovers(slide, elementsToSelect, selectionBoxGenerator);
				slide.RaiseEvent(new RoutedCollectionChangedEventArgs<Element>(Slide.SelectionChangedEvent, slide.GetSelectedElements()));
			}
		}

		public static void AppendUnselect([NotNull] this Slide slide, [NotNull] params Element[] elementsToUnselect)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elementsToUnselect == null)
			{
				throw new ArgumentNullException("elementsToUnselect");
			}
			elementsToUnselect = elementsToUnselect.TopGroupElements();
			ReadOnlyCollection<Element> selectedElements = slide.GetSelectedElements();
			elementsToUnselect = elementsToUnselect.Where(selectedElements.Contains).ToArray();
			if (!elementsToUnselect.Any())
			{
				return;
			}
			ISelectionBoxGenerator selectionBoxGenerator = elementsToUnselect.First().GetSelectionBoxGenerator();
			if (selectionBoxGenerator != null)
			{
				if (!slide.IsLoaded)
				{
					throw new InvalidOperationException("不能取消选择一个并没有显示的 Slide 上的元素。");
				}
				UnselectAndRemoveCovers(slide, elementsToUnselect, selectionBoxGenerator);
				slide.RaiseEvent(new RoutedCollectionChangedEventArgs<Element>(Slide.SelectionChangedEvent, selectedElements));
			}
		}

		public static void AppendSwitchingSelection([NotNull] this Slide slide, [NotNull] Element[] elementToSwitch)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elementToSwitch == null)
			{
				throw new ArgumentNullException("elementToSwitch");
			}
			elementToSwitch = elementToSwitch.TopGroupElements();
			if (elementToSwitch.Any((Element x) => !slide.GetSelectedElements().Contains(x)))
			{
				slide.AppendSelect(FilterHitTest(elementToSwitch).ToArray());
			}
			else
			{
				slide.AppendUnselect(elementToSwitch);
			}
		}

		public static void ClearAndSelect([NotNull] this Slide slide, [NotNull] params Element[] elementsToSelect)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elementsToSelect == null)
			{
				throw new ArgumentNullException("elementsToSelect");
			}
			Ensure.ItemIsNotNull(elementsToSelect, "elementsToSelect");
			elementsToSelect = FilterHitTest(elementsToSelect).TopGroupElements();
			Element[] array = slide.GetSelectedElements().Except(elementsToSelect).ToArray();
			elementsToSelect = elementsToSelect.Except(slide.GetSelectedElements()).ToArray();
			if (!array.Any() && !elementsToSelect.Any())
			{
				return;
			}
			UIPerformance.GetPerformanceCollector(slide).CollectUIDelay("Select", elementsToSelect.Any() ? "Select" : "Unselect", new OperatingElementsPerformanceBag(slide, elementsToSelect));
			ISelectionBoxGenerator selectionBoxGenerator = slide.GetSelectionBoxGenerator();
			if (selectionBoxGenerator != null)
			{
				if (!slide.IsLoaded)
				{
					throw new InvalidOperationException("不能选择一个并没有显示的 Slide 上的元素。");
				}
				UnselectAndRemoveCovers(slide, array, selectionBoxGenerator);
				SelectAndAddCovers(slide, elementsToSelect, selectionBoxGenerator);
				slide.RaiseEvent(new RoutedCollectionChangedEventArgs<Element>(Slide.SelectionChangedEvent, slide.GetSelectedElements()));
			}
		}

		public static void Reselect([NotNull] this Slide slide, [NotNull] params Element[] elementsToSelect)
		{
			slide.Reselect((IList<Element>)elementsToSelect);
		}

		public static void Reselect([NotNull] this Slide slide, [NotNull] IList<Element> elementsToSelect)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (elementsToSelect == null)
			{
				throw new ArgumentNullException("elementsToSelect");
			}
			elementsToSelect = FilterHitTest(elementsToSelect).TopGroupElements();
			Element[] array = slide.GetSelectedElements().ToArray();
			if (!array.Any() && !elementsToSelect.Any())
			{
				return;
			}
			UIPerformance.GetPerformanceCollector(slide).CollectUIDelay("Select", "Reselect", new OperatingElementsPerformanceBag(slide, elementsToSelect));
			ISelectionBoxGenerator selectionBoxGenerator = slide.GetSelectionBoxGenerator();
			if (selectionBoxGenerator != null)
			{
				if (!slide.IsLoaded)
				{
					throw new InvalidOperationException("不能重新选择一个并没有显示的 Slide 上的元素。");
				}
				UnselectAndRemoveCovers(slide, array, selectionBoxGenerator, elementsToSelect);
				SelectAndAddCovers(slide, elementsToSelect, selectionBoxGenerator, array);
				if (!elementsToSelect.SequenceEqual(array))
				{
					slide.RaiseEvent(new RoutedCollectionChangedEventArgs<Element>(Slide.SelectionChangedEvent, slide.GetSelectedElements()));
				}
			}
		}

		private static void SelectAndAddCovers(Slide slide, IList<Element> elementsToSelect, ISelectionBoxGenerator generator, IList<Element> elementsToUnselect = null)
		{
			if (!elementsToSelect.Any())
			{
				return;
			}
			elementsToSelect = (from element in elementsToSelect.Where(GetCanSelect)
				where Slide.GetSlide(element) == slide
				select element).ToList();
			slide.GetEditableSelectedElements().AddRange(elementsToSelect);
			generator.AddSelectionBoxes(slide, elementsToSelect);
			foreach (Element item in elementsToSelect.Where((Element x) =>
			{
				IList<Element> list = elementsToUnselect;
				return list == null || !list.Contains(x);
			}))
			{
				(item as ISelectionSensitive)?.Selected();
			}
		}

		private static void UnselectAndRemoveCovers(Slide slide, IList<Element> elementsToUnselect, ISelectionBoxGenerator generator, IList<Element> elementsToSelect = null)
		{
			if (!elementsToUnselect.Any())
			{
				return;
			}
			if (elementsToUnselect.Any((Element x) => x.GetIsTransforming()))
			{
				Mouse.Capture(null);
				Stylus.Capture(null);
			}
			if (elementsToSelect != null)
			{
				foreach (IGroupElement item in elementsToSelect.OfType<IGroupElement>())
				{
					item.RemoveInnerSelectionBoxes();
				}
			}
			slide.GetEditableSelectedElements().RemoveAll(elementsToUnselect.Contains);
			generator.RemoveSelectionBoxes(slide, elementsToUnselect);
			foreach (Element item2 in elementsToUnselect.Where((Element x) =>
			{
				IList<Element> list = elementsToSelect;
				return list == null || !list.Contains(x);
			}))
			{
				(item2 as ISelectionSensitive)?.Unselected();
			}
		}

		public static void AddSelectionBoxes(this ISelectionBoxGenerator generator, Slide slide, IList<Element> elementsToSelect)
		{
			generator.GenerateSelectionBox(slide, elementsToSelect);
		}

		public static void RemoveSelectionBoxes(this ISelectionBoxGenerator generator, Slide slide, IList<Element> elementsToUnselect)
		{
			generator.DegenerateSelectionBox(slide, elementsToUnselect);
		}

		public static void RefreshSelectionBoxes(this Slide slide)
		{
			SelectionBoxLayer selectionBoxLayer = slide.GetSelectionBoxLayer();
			if (selectionBoxLayer != null && selectionBoxLayer.IsAttached)
			{
				selectionBoxLayer.SetOutAdornerVisibility(Visibility.Visible);
				selectionBoxLayer.SetOutAdornerOpacity(1.0);
			}
			slide.GetSelectionOperators().ForEach((ISelectionOperator x) =>
			{
				x.OnSelectionUpdated();
			});
		}

		public static void HideSelectionBoxes(this Slide slide)
		{
			slide.HideSelectionBoxes(true);
		}

		public static void HideSelectionBoxes(this Slide slide, bool visibilityOrOpacity)
		{
			SelectionBoxLayer selectionBoxLayer = slide.GetSelectionBoxLayer();
			if (selectionBoxLayer != null && selectionBoxLayer.IsAttached)
			{
				if (visibilityOrOpacity)
				{
					selectionBoxLayer.SetOutAdornerVisibility(Visibility.Hidden);
				}
				else
				{
					selectionBoxLayer.SetOutAdornerOpacity(0.0);
				}
			}
		}

		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		private static void VerifyElementsCanSelect(IList<Element> elements)
		{
			if (elements.Any())
			{
				AdornerLayer.GetAdornerLayer(Slide.GetSlide(elements.First()));
			}
		}

		private static IEnumerable<Element> FilterHitTest(IEnumerable<Element> elements)
		{
			return elements.Where((Element x) => x.IsHitTestVisible && x.Visibility == Visibility.Visible);
		}
	}
}
