using System;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class ShadowBorder : DependencyObject
	{
		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ShadowBorder), new PropertyMetadata(default(Color)));

		public static readonly DependencyProperty InactiveColorProperty = DependencyProperty.Register("InactiveColor", typeof(Color), typeof(ShadowBorder), new PropertyMetadata(default(Color)));

		public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(Thickness), typeof(ShadowBorder), new PropertyMetadata(default(Thickness), RadiusPropertyChangedCallback));

		public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(ShadowBorder), new PropertyMetadata(0.6));

		public static readonly DependencyProperty ShrinkProperty = DependencyProperty.Register("Shrink", typeof(Thickness), typeof(ShadowBorder), new PropertyMetadata(new Thickness(0.5), ShrinkPropertyChangedCallback));

		public static readonly DependencyProperty WindowOutterBorderThicknessProperty = DependencyProperty.Register("WindowOutterBorderThickness", typeof(Thickness), typeof(ShadowBorder), new PropertyMetadata(default(Thickness)));

		public static readonly DependencyProperty WindowOutterBorderColorProperty = DependencyProperty.Register("WindowOutterBorderColor", typeof(Color), typeof(ShadowBorder), new PropertyMetadata(default(Color)));

		public static readonly DependencyProperty CanResizeProperty = DependencyProperty.Register("CanResize", typeof(bool), typeof(ShadowBorder), new PropertyMetadata(false));

		public static readonly RoutedEvent RadiusChangedEvent = EventManager.RegisterRoutedEvent("RadiusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ShadowBorder));

		public static readonly RoutedEvent ShrinkChangedEvent = EventManager.RegisterRoutedEvent("ShrinkChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ShadowBorder));

		public static readonly DependencyProperty ShadowBorderProperty = DependencyProperty.RegisterAttached("ShadowBorder", typeof(ShadowBorder), typeof(ShadowBorder), new PropertyMetadata(null, ShadowBorderPropertyChangedCallback));

		public bool CanResize
		{
			get
			{
				return (bool)GetValue(CanResizeProperty);
			}
			set
			{
				SetValue(CanResizeProperty, value);
			}
		}

		public Color WindowOutterBorderColor
		{
			get
			{
				return (Color)GetValue(WindowOutterBorderColorProperty);
			}
			set
			{
				SetValue(WindowOutterBorderColorProperty, value);
			}
		}

		public Thickness WindowOutterBorderThickness
		{
			get
			{
				return (Thickness)GetValue(WindowOutterBorderThicknessProperty);
			}
			set
			{
				SetValue(WindowOutterBorderThicknessProperty, value);
			}
		}

		public Color Color
		{
			get
			{
				return (Color)GetValue(ColorProperty);
			}
			set
			{
				SetValue(ColorProperty, value);
			}
		}

		public Color InactiveColor
		{
			get
			{
				return (Color)GetValue(InactiveColorProperty);
			}
			set
			{
				SetValue(InactiveColorProperty, value);
			}
		}

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

		public Thickness Shrink
		{
			get
			{
				return (Thickness)GetValue(ShrinkProperty);
			}
			set
			{
				SetValue(ShrinkProperty, value);
			}
		}

		public double Opacity
		{
			get
			{
				return (double)GetValue(OpacityProperty);
			}
			set
			{
				SetValue(OpacityProperty, value);
			}
		}

		public Func<double, double> GradientFunc { get; set; } = DefaultGradientFunc;

		internal Window Owner { get; set; }

		private static void RadiusPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ShadowBorder shadowBorder = (ShadowBorder)obj;
			shadowBorder.Owner?.RaiseEvent(new RoutedEventArgs(RadiusChangedEvent, shadowBorder));
		}

		private static void ShrinkPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ShadowBorder shadowBorder = (ShadowBorder)obj;
			shadowBorder.Owner?.RaiseEvent(new RoutedEventArgs(ShrinkChangedEvent, shadowBorder));
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
					((ShadowController)window.GetValue(ShadowController.ShadowControllerProperty)).Close();
					((ShadowBorder)e.OldValue).Owner = null;
					window.SetValue(ShadowController.ShadowControllerProperty, null);
				}
				if (e.NewValue != null)
				{
					((ShadowBorder)e.NewValue).Owner = window;
					ShadowController value = new ShadowController(window);
					window.SetValue(ShadowController.ShadowControllerProperty, value);
				}
			}
		}

		public static void SetShadowBorder(Window element, ShadowBorder value)
		{
			element.SetValue(ShadowBorderProperty, value);
		}

		public static ShadowBorder GetShadowBorder(Window element)
		{
			return (ShadowBorder)element.GetValue(ShadowBorderProperty);
		}

		private static double DefaultGradientFunc(double x)
		{
			return Math.Pow(x, 2.0);
		}
	}
}
