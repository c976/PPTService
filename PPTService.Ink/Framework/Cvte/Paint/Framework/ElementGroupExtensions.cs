using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Linq;
using Cvte.Paint.Framework.Elements;
using Cvte.Paint.Framework.Operations;

namespace Cvte.Paint.Framework
{
	public static class ElementGroupExtensions
	{
		[Obsolete("请使用 ElementOperation Group([NotNull] this Slide slide, IEnumerable<Element> elements,bool isAutoSelect=true) 代替，因为这个方法需要自己处理撤销重做，组合的撤销需要处理时机问题，我不觉得你能用对")]
		public static IGroupElement Group([NotNull] this IEnumerable<Element> elements)
		{
			IGroupElement interfaceInstance = InterfaceCreator.GetInterfaceInstance<IGroupElement>(new object[1] { elements });
			if (interfaceInstance == null)
			{
				throw new InvalidOperationException("没有找到 IGroupElement 元素的构造方法，请在 InterfaceCreator 注册相应的方法\r\n可以引用 Cvte.Paint.Features 之后使用 InterfaceCreator.RegisterInterfaceInstance(() => (IGroupElement) new GroupElement()); 告诉接口如何添加组合");
			}
			interfaceInstance.Group();
			return interfaceInstance;
		}

		public static ElementOperation Group([NotNull] this Slide slide, IEnumerable<Element> elements, bool isAutoSelect = true)
		{
			IGroupElement interfaceInstance = InterfaceCreator.GetInterfaceInstance<IGroupElement>(new object[1] { elements });
			if (interfaceInstance == null)
			{
				throw new InvalidOperationException("没有找到 IGroupElement 元素的构造方法，请在 InterfaceCreator 注册相应的方法\r\n可以引用 Cvte.Paint.Features 之后使用 InterfaceCreator.RegisterInterfaceInstance(() => (IGroupElement) new GroupElement()); 告诉接口如何添加组合");
			}
			ElementGroupingResult elementGroupResult = interfaceInstance.Group(true);
			if (isAutoSelect && slide.IsLoaded)
			{
				slide.AppendSelect((Element)interfaceInstance);
			}
			return new ElementGroupOperation(elementGroupResult);
		}

		public static ElementOperation Ungroup([NotNull] this IGroupElement groupElement, bool isAutoSelect)
		{
			return new ElementGroupOperation(groupElement.Ungroup(true, isAutoSelect));
		}

		[Obsolete("请使用 ElementOperation Group([NotNull] this Slide slide, IEnumerable<Element> elements,bool isAutoSelect=true) 代替，因为这个方法需要自己处理撤销重做，组合的撤销需要处理时机问题，我不觉得你能用对")]
		public static IGroupElement Group<T>([NotNull] this IEnumerable<Element> elements) where T : IGroupElement, new()
		{
			object obj = new T();
			((IGroupElement)obj).Group();
			return (IGroupElement)obj;
		}

		[Obsolete("请使用 ElementOperation Ungroup([NotNull] this IGroupElement groupElement, bool isAutoSelect) 代替，这个方法需要自己处理撤销重做")]
		public static IList<Element> Ungroup([NotNull] this IEnumerable<Element> elements)
		{
			List<Element> ungroupedElements = new List<Element>();
			elements.ForEach((Element g) =>
			{
				IGroupElement groupElement = g as IGroupElement;
				if (groupElement == null)
				{
					ungroupedElements.Add(g);
				}
				else
				{
					ungroupedElements.AddRange(groupElement.Ungroup());
				}
			});
			return ungroupedElements;
		}

		public static bool IsGrouped([NotNull] this Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return element != element.TopGroupElement();
		}

		public static bool IsExistsGroupInnerSelected(this IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			return elements.Any((Element e) => e.IsExistsGroupInnerSelected());
		}

		public static bool IsExistsGroupInnerSelected(this Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			GroupElementBase groupElementBase = element as GroupElementBase;
			if (groupElementBase != null && groupElementBase.InnerSelections.Any())
			{
				return true;
			}
			if (element.IsGroupInnerSelected())
			{
				return true;
			}
			return false;
		}

		public static Element TopGroupElement(this DependencyObject d)
		{
			return d.TopVisualAncestor<Element, Slide>();
		}

		public static Element TopGroupElement([NotNull] this Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			FrameworkElement frameworkElement = element;
			while (true)
			{
				FrameworkElement frameworkElement2 = frameworkElement.Parent as FrameworkElement;
				if (frameworkElement2 is ElementCanvas)
				{
					return (Element)frameworkElement;
				}
				if (frameworkElement2 == null)
				{
					break;
				}
				frameworkElement = frameworkElement2;
			}
			return (Element)frameworkElement;
		}

		public static Element[] TopGroupElements([NotNull] this IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			return elements.Select((Element x) => x.TopVisualAncestor<Element, Slide>()).Distinct().ToArray();
		}

		public static Element FindGroupControlledChildOrRootElement(Element inner)
		{
			GroupElementBase groupElementBase = inner.TopGroupElement() as GroupElementBase;
			if (groupElementBase != null)
			{
				return FindDirectChild(groupElementBase, inner) ?? inner;
			}
			return inner;
		}

		internal static Element FindDirectChild(GroupElementBase group, Element inner)
		{
			Element element = inner;
			bool flag = group.Elements.Contains(element);
			while (!flag)
			{
				Element element2 = FindOwnerElement(element, false);
				if (element2 == null || element2 == group)
				{
					return null;
				}
				flag = group.Elements.Contains(element2);
				element = element2;
			}
			return element;
		}

		private static Element FindOwnerElement(object source, bool includeSelf)
		{
			DependencyObject dependencyObject = source as DependencyObject;
			if (dependencyObject != null)
			{
				Element element;
				while (true)
				{
					if (dependencyObject == null || dependencyObject is Slide || dependencyObject is Board)
					{
						return null;
					}
					element = dependencyObject as Element;
					if (element != null)
					{
						if (dependencyObject != source)
						{
							break;
						}
						if (includeSelf)
						{
							return element;
						}
						dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
					}
					else
					{
						dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
					}
				}
				return element;
			}
			return null;
		}

		public static IEnumerable<Element> EnumerateWithGroup([NotNull] this IEnumerable<Element> elements)
		{
			foreach (Element element in elements)
			{
				yield return element;
				IGroupElement groupElement = element as IGroupElement;
				if (groupElement == null)
				{
					continue;
				}
				foreach (Element item in groupElement.Elements.EnumerateWithGroup())
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<Element> EnumerateWithGroup([NotNull] this Element element)
		{
			return new Element[1] { element }.EnumerateWithGroup();
		}

		public static IEnumerable<Element> EnumerateInner([NotNull] this IEnumerable<Element> elements)
		{
			foreach (Element element in elements)
			{
				IGroupElement groupElement = element as IGroupElement;
				if (groupElement == null)
				{
					yield return element;
					continue;
				}
				foreach (Element item in groupElement.Elements.EnumerateInner())
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<Element> EnumerateInner([NotNull] this Element element)
		{
			return new Element[1] { element }.EnumerateInner();
		}

		public static IEnumerable<Element> EnumerateInnerSelection([NotNull] this Element element, bool enumerateIfNoneSelected = true)
		{
			IGroupElement groupElement = element as IGroupElement;
			if (groupElement == null)
			{
				yield return element;
				yield break;
			}
			if (groupElement.InnerSelections == null || !groupElement.InnerSelections.Any())
			{
				if (enumerateIfNoneSelected)
				{
					foreach (Element item in groupElement.Elements.EnumerateInner())
					{
						yield return item;
					}
				}
				else
				{
					yield return element;
				}
				yield break;
			}
			foreach (Element innerSelection in groupElement.InnerSelections)
			{
				yield return innerSelection;
			}
		}

		[CanBeNull]
		private static TTarget TopVisualAncestor<TTarget, TTop>([NotNull] this DependencyObject visual) where TTarget : Visual where TTop : Visual
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			TTarget result = null;
			for (DependencyObject dependencyObject = visual; dependencyObject != null; dependencyObject = VisualTreeHelper.GetParent(dependencyObject))
			{
				TTarget val = dependencyObject as TTarget;
				if (dependencyObject as TTop != null)
				{
					break;
				}
				if (val != null)
				{
					result = val;
				}
			}
			return result;
		}

		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		private static void VerifyElementsCanGroup(IEnumerable<Element> elements)
		{
			if (elements.Any())
			{
				Slide.GetSlide(elements.First());
			}
		}

		public static bool IsGroupInnerSelected(this Element element)
		{
			return GroupElementBase.GetIsGroupInnerSelected(element);
		}

		public static void SetIsGroupInnerSelected(this Element element, bool value)
		{
			GroupElementBase.SetIsGroupInnerSelected(element, value);
		}

		public static bool IsGroupInnerElement(this Element element)
		{
			return GroupElementBase.GetIsGroupInnerElement(element);
		}

		public static void SetIsGroupInnerElement(this Element element, bool value)
		{
			GroupElementBase.SetIsGroupInnerElement(element, value);
		}
	}
}
