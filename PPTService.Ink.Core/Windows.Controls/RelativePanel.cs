using System;
using System.Windows;
using System.Windows.Controls;

namespace PPTService.Ink.Core.Windows.Controls
{
	public class RelativePanel : Panel
	{
		public static readonly DependencyProperty AboveProperty = DependencyProperty.RegisterAttached("Above", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty BelowProperty = DependencyProperty.RegisterAttached("Below", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty LeftOfProperty = DependencyProperty.RegisterAttached("LeftOf", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty RightOfProperty = DependencyProperty.RegisterAttached("RightOf", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignLeftWithProperty = DependencyProperty.RegisterAttached("AlignLeftWith", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignTopWithProperty = DependencyProperty.RegisterAttached("AlignTopWith", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignRightWithProperty = DependencyProperty.RegisterAttached("AlignRightWith", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignBottomWithProperty = DependencyProperty.RegisterAttached("AlignBottomWith", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignHorizontalCenterWithProperty = DependencyProperty.RegisterAttached("AlignHorizontalCenterWith", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignVerticalCenterWithProperty = DependencyProperty.RegisterAttached("AlignVerticalCenterWith", typeof(UIElement), typeof(RelativePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignLeftWithPanelProperty = DependencyProperty.RegisterAttached("AlignLeftWithPanel", typeof(bool), typeof(RelativePanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignTopWithPanelProperty = DependencyProperty.RegisterAttached("AlignTopWithPanel", typeof(bool), typeof(RelativePanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignRightWithPanelProperty = DependencyProperty.RegisterAttached("AlignRightWithPanel", typeof(bool), typeof(RelativePanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignBottomWithPanelProperty = DependencyProperty.RegisterAttached("AlignBottomWithPanel", typeof(bool), typeof(RelativePanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignHorizontalCenterWithPanelProperty = DependencyProperty.RegisterAttached("AlignHorizontalCenterWithPanel", typeof(bool), typeof(RelativePanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		public static readonly DependencyProperty AlignVerticalCenterWithPanelProperty = DependencyProperty.RegisterAttached("AlignVerticalCenterWithPanel", typeof(bool), typeof(RelativePanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

		protected override Size MeasureOverride(Size availableSize)
		{
			Size result = default(Size);
			foreach (UIElement internalChild in InternalChildren)
			{
				internalChild.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				result.Width = Math.Max(result.Width, internalChild.DesiredSize.Width);
				result.Height = Math.Max(result.Height, internalChild.DesiredSize.Height);
			}
			result.Width = Math.Min(result.Width, availableSize.Width);
			result.Height = Math.Min(result.Height, availableSize.Height);
			return result;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement internalChild in InternalChildren)
			{
				RelativeArrange(finalSize, internalChild);
			}
			return finalSize;
		}

		private void RelativeArrange(Size ownSize, UIElement child)
		{
			double? num = GetRightOf(child)?.TranslatePoint(new Point(GetRightOf(child).RenderSize.Width, 0.0), this).X;
			double? num2 = GetBelow(child)?.TranslatePoint(new Point(0.0, GetBelow(child).RenderSize.Height), this).Y;
			double? num3 = GetLeftOf(child)?.TranslatePoint(default(Point), this).X;
			double? num4 = GetAbove(child)?.TranslatePoint(default(Point), this).Y;
			UIElement alignLeftWith = GetAlignLeftWith(child);
			num = ((alignLeftWith != null) ? new double?(alignLeftWith.TranslatePoint(default(Point), this).X) : num);
			UIElement alignTopWith = GetAlignTopWith(child);
			num2 = ((alignTopWith != null) ? new double?(alignTopWith.TranslatePoint(default(Point), this).Y) : num2);
			UIElement alignRightWith = GetAlignRightWith(child);
			num3 = ((alignRightWith != null) ? new double?(alignRightWith.TranslatePoint(new Point(GetAlignRightWith(child).RenderSize.Width, 0.0), this).X) : num3);
			UIElement alignBottomWith = GetAlignBottomWith(child);
			num4 = ((alignBottomWith != null) ? new double?(alignBottomWith.TranslatePoint(new Point(0.0, GetAlignBottomWith(child).RenderSize.Height), this).Y) : num4);
			num = (GetAlignLeftWithPanel(child) ? new double?(0.0) : num);
			num2 = (GetAlignTopWithPanel(child) ? new double?(0.0) : num2);
			num3 = (GetAlignRightWithPanel(child) ? new double?(ownSize.Width) : num3);
			num4 = (GetAlignBottomWithPanel(child) ? new double?(ownSize.Height) : num4);
			num = ((GetAlignHorizontalCenterWith(child) == null) ? num : new double?((GetAlignHorizontalCenterWith(child).RenderSize.Width - child.DesiredSize.Width) / 2.0 + GetAlignHorizontalCenterWith(child).TranslatePoint(default(Point), this).X));
			num3 = ((GetAlignHorizontalCenterWith(child) == null) ? num3 : new double?(num.Value + child.DesiredSize.Width));
			num2 = ((GetAlignVerticalCenterWith(child) == null) ? num2 : new double?((GetAlignVerticalCenterWith(child).RenderSize.Height - child.DesiredSize.Height) / 2.0 + GetAlignVerticalCenterWith(child).TranslatePoint(default(Point), this).Y));
			num4 = ((GetAlignVerticalCenterWith(child) == null) ? num4 : new double?(num2.Value + child.DesiredSize.Height));
			num = ((!GetAlignHorizontalCenterWithPanel(child)) ? num : new double?((ownSize.Width - child.DesiredSize.Width) / 2.0));
			num3 = ((!GetAlignHorizontalCenterWithPanel(child)) ? num3 : new double?(num.Value + child.DesiredSize.Width));
			num2 = ((!GetAlignVerticalCenterWithPanel(child)) ? num2 : new double?((ownSize.Height - child.DesiredSize.Height) / 2.0));
			num4 = ((!GetAlignVerticalCenterWithPanel(child)) ? num4 : new double?(num2.Value + child.DesiredSize.Height));
			double? num5 = null;
			double? num6 = null;
			double? num7 = null;
			double? num8 = null;
			if (num.HasValue && num3.HasValue)
			{
				if (num.Value < num3.Value)
				{
					num7 = num3.Value - num.Value;
					num5 = num.Value;
				}
			}
			else if (num.HasValue)
			{
				num7 = child.DesiredSize.Width;
				num5 = num.Value;
			}
			else if (num3.HasValue)
			{
				num7 = child.DesiredSize.Width;
				num5 = num3.Value - num7.Value;
			}
			if (num2.HasValue && num4.HasValue)
			{
				if (num2.Value < num4.Value)
				{
					num8 = num4.Value - num2.Value;
					num6 = num2.Value;
				}
			}
			else if (num2.HasValue)
			{
				num8 = child.DesiredSize.Height;
				num6 = num2.Value;
			}
			else if (num4.HasValue)
			{
				num8 = child.DesiredSize.Height;
				num6 = num4.Value - num8.Value;
			}
			if (num5.HasValue && num6.HasValue)
			{
				child.Arrange(new Rect(num5.Value, num6.Value, num7.Value, num8.Value));
			}
			else if (num5.HasValue)
			{
				child.Arrange(new Rect(num5.Value, 0.0, num7.Value, child.DesiredSize.Height));
			}
			else if (num6.HasValue)
			{
				child.Arrange(new Rect(0.0, num6.Value, child.DesiredSize.Width, num8.Value));
			}
			else if (!num.HasValue && !num2.HasValue && !num3.HasValue && !num4.HasValue)
			{
				child.Arrange(new Rect(0.0, 0.0, child.DesiredSize.Width, child.DesiredSize.Height));
			}
			else if (num.HasValue && num3.HasValue)
			{
				child.Arrange(new Rect(num.Value, 0.0, 0.0, 0.0));
			}
			else if (num2.HasValue && num4.HasValue)
			{
				child.Arrange(new Rect(0.0, num2.Value, 0.0, 0.0));
			}
			else
			{
				child.Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
			}
		}

		public static void SetAbove(DependencyObject element, UIElement value)
		{
			element.SetValue(AboveProperty, value);
		}

		public static UIElement GetAbove(DependencyObject element)
		{
			return (UIElement)element.GetValue(AboveProperty);
		}

		public static void SetBelow(DependencyObject element, UIElement value)
		{
			element.SetValue(BelowProperty, value);
		}

		public static UIElement GetBelow(DependencyObject element)
		{
			return (UIElement)element.GetValue(BelowProperty);
		}

		public static void SetLeftOf(DependencyObject element, UIElement value)
		{
			element.SetValue(LeftOfProperty, value);
		}

		public static UIElement GetLeftOf(DependencyObject element)
		{
			return (UIElement)element.GetValue(LeftOfProperty);
		}

		public static void SetRightOf(DependencyObject element, UIElement value)
		{
			element.SetValue(RightOfProperty, value);
		}

		public static UIElement GetRightOf(DependencyObject element)
		{
			return (UIElement)element.GetValue(RightOfProperty);
		}

		public static void SetAlignLeftWith(DependencyObject element, UIElement value)
		{
			element.SetValue(AlignLeftWithProperty, value);
		}

		public static UIElement GetAlignLeftWith(DependencyObject element)
		{
			return (UIElement)element.GetValue(AlignLeftWithProperty);
		}

		public static void SetAlignTopWith(DependencyObject element, UIElement value)
		{
			element.SetValue(AlignTopWithProperty, value);
		}

		public static UIElement GetAlignTopWith(DependencyObject element)
		{
			return (UIElement)element.GetValue(AlignTopWithProperty);
		}

		public static void SetAlignRightWith(DependencyObject element, UIElement value)
		{
			element.SetValue(AlignRightWithProperty, value);
		}

		public static UIElement GetAlignRightWith(DependencyObject element)
		{
			return (UIElement)element.GetValue(AlignRightWithProperty);
		}

		public static void SetAlignBottomWith(DependencyObject element, UIElement value)
		{
			element.SetValue(AlignBottomWithProperty, value);
		}

		public static UIElement GetAlignBottomWith(DependencyObject element)
		{
			return (UIElement)element.GetValue(AlignBottomWithProperty);
		}

		public static void SetAlignHorizontalCenterWith(DependencyObject element, UIElement value)
		{
			element.SetValue(AlignHorizontalCenterWithProperty, value);
		}

		public static UIElement GetAlignHorizontalCenterWith(DependencyObject element)
		{
			return (UIElement)element.GetValue(AlignHorizontalCenterWithProperty);
		}

		public static void SetAlignVerticalCenterWith(DependencyObject element, UIElement value)
		{
			element.SetValue(AlignVerticalCenterWithProperty, value);
		}

		public static UIElement GetAlignVerticalCenterWith(DependencyObject element)
		{
			return (UIElement)element.GetValue(AlignVerticalCenterWithProperty);
		}

		public static void SetAlignLeftWithPanel(DependencyObject element, bool value)
		{
			element.SetValue(AlignLeftWithPanelProperty, value);
		}

		public static bool GetAlignLeftWithPanel(DependencyObject element)
		{
			return (bool)element.GetValue(AlignLeftWithPanelProperty);
		}

		public static void SetAlignTopWithPanel(DependencyObject element, bool value)
		{
			element.SetValue(AlignTopWithPanelProperty, value);
		}

		public static bool GetAlignTopWithPanel(DependencyObject element)
		{
			return (bool)element.GetValue(AlignTopWithPanelProperty);
		}

		public static void SetAlignRightWithPanel(DependencyObject element, bool value)
		{
			element.SetValue(AlignRightWithPanelProperty, value);
		}

		public static bool GetAlignRightWithPanel(DependencyObject element)
		{
			return (bool)element.GetValue(AlignRightWithPanelProperty);
		}

		public static void SetAlignBottomWithPanel(DependencyObject element, bool value)
		{
			element.SetValue(AlignBottomWithPanelProperty, value);
		}

		public static bool GetAlignBottomWithPanel(DependencyObject element)
		{
			return (bool)element.GetValue(AlignBottomWithPanelProperty);
		}

		public static void SetAlignHorizontalCenterWithPanel(DependencyObject element, bool value)
		{
			element.SetValue(AlignHorizontalCenterWithPanelProperty, value);
		}

		public static bool GetAlignHorizontalCenterWithPanel(DependencyObject element)
		{
			return (bool)element.GetValue(AlignHorizontalCenterWithPanelProperty);
		}

		public static void SetAlignVerticalCenterWithPanel(DependencyObject element, bool value)
		{
			element.SetValue(AlignVerticalCenterWithPanelProperty, value);
		}

		public static bool GetAlignVerticalCenterWithPanel(DependencyObject element)
		{
			return (bool)element.GetValue(AlignVerticalCenterWithPanelProperty);
		}
	}
}
