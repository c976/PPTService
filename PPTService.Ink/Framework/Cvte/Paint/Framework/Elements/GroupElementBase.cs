using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Level;
using Cvte.Windows;

namespace Cvte.Paint.Framework.Elements
{
	public abstract class GroupElementBase : Element, IGroupElement, IElementContainer
	{
		public static readonly DependencyProperty IsGroupInnerSelectedProperty = DependencyProperty.RegisterAttached("IsGroupInnerSelected", typeof(bool), typeof(Element), new PropertyMetadata(false, GroupInnerSelectedChanged));

		internal static readonly DependencyProperty IsGroupInnerElementProperty = DependencyProperty.RegisterAttached("IsGroupInnerElement", typeof(bool), typeof(GroupElementBase), new PropertyMetadata(false));

		public abstract IEnumerable<Element> InnerSelections { get; }

		[Obsolete("请使用InnerSelections代替", true)]
		public abstract Element InnerSelection { get; }

		public abstract ICollection<Element> Elements { get; }

		public abstract void Group();

		public abstract void InnerSelect([CanBeNull] Element innerElement);

		public abstract void RemoveInnerSelectionBoxes();

		[Obsolete("请使用RemoveInnerSelectionBoxes代替", true)]
		public abstract void RemoveInnerSelectionBox();

		public abstract IList<Element> Ungroup();

		internal static bool GetIsGroupInnerSelected(Element element)
		{
			return (bool)element.GetValue(IsGroupInnerSelectedProperty);
		}

		internal static void SetIsGroupInnerSelected(Element element, bool value)
		{
			element.SetValue(IsGroupInnerSelectedProperty, value);
		}

		private static void GroupInnerSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((Element)sender).VisualAncestor<GroupElementBase>()?.OnIsGroupInnerSelectedChanged(sender, e);
		}

		protected abstract void OnIsGroupInnerSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e);

		internal static bool GetIsGroupInnerElement(Element element)
		{
			return (bool)element.GetValue(IsGroupInnerElementProperty);
		}

		internal static void SetIsGroupInnerElement(Element element, bool value)
		{
			element.SetValue(IsGroupInnerElementProperty, value);
		}

		IList<ILevelElement> IElementContainer.GetLevelElementList()
		{
			return Elements.Cast<ILevelElement>().ToList();
		}

		public abstract ElementGroupingResult Group(bool isAutoAddToSlide);

		public abstract ElementGroupingResult Ungroup(bool isNewLevelRequired, bool isAutoSelect = true);
	}
}
