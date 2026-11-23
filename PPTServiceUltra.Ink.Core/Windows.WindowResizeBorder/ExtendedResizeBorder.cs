using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.WindowResizeBorder
{
	public class ExtendedResizeBorder : DependencyObject
	{
		public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(Thickness), typeof(ExtendedResizeBorder), new PropertyMetadata(default(Thickness), RadiusPropertyChangedCallback));

		public static readonly RoutedEvent RadiusChangedEvent = EventManager.RegisterRoutedEvent("RadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedResizeBorder));

		public static readonly DependencyProperty ExtendedResizeBorderProperty = DependencyProperty.RegisterAttached("ExtendedResizeBorder", typeof(ExtendedResizeBorder), typeof(ExtendedResizeBorder), new PropertyMetadata(null, ShadowBorderPropertyChangedCallback));

		public Thickness Radius
		{
			get
			{
				return (Thickness)GetValue(RadiusProperty);
			}
			set
			{
				SetValue(RadiusProperty, value);
			}
		}

		internal Window Owner { get; set; }

		private static void RadiusPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ExtendedResizeBorder extendedResizeBorder = (ExtendedResizeBorder)obj;
			extendedResizeBorder.Owner?.RaiseEvent(new RoutedEventArgs(RadiusChangedEvent, extendedResizeBorder));
		}

		public static void AddRadiusChangedHandler(DependencyObject d, RoutedEventHandler handler)
		{
			(d as UIElement)?.AddHandler(RadiusChangedEvent, handler);
		}

		public static void RemoveRadiusChangedHandler(DependencyObject d, RoutedEventHandler handler)
		{
			(d as UIElement)?.RemoveHandler(RadiusChangedEvent, handler);
		}

		private static void ShadowBorderPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window window = dependencyObject as Window;
			if (window != null && (e.OldValue != null || e.NewValue != null))
			{
				if (e.OldValue != null)
				{
					((ExtendedResizeController)window.GetValue(ExtendedResizeController.ShadowControllerProperty)).Close();
					((ExtendedResizeBorder)e.OldValue).Owner = null;
					window.SetValue(ExtendedResizeController.ShadowControllerProperty, null);
				}
				if (e.NewValue != null)
				{
					((ExtendedResizeBorder)e.NewValue).Owner = window;
					ExtendedResizeController value = new ExtendedResizeController(window);
					window.SetValue(ExtendedResizeController.ShadowControllerProperty, value);
				}
			}
		}

		public static void SetExtendedResizeBorder(Window element, ExtendedResizeBorder value)
		{
			element.SetValue(ExtendedResizeBorderProperty, value);
		}

		public static ExtendedResizeBorder GetExtendedResizeBorder(Window element)
		{
			return (ExtendedResizeBorder)element.GetValue(ExtendedResizeBorderProperty);
		}
	}
}
