using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;

namespace PPTService
{
	public class AdaptiveTrigger : TriggerBase<FrameworkElement>
	{
		public static readonly DependencyProperty MinWindowWidthProperty = DependencyProperty.Register("MinWindowWidth", typeof(double), typeof(AdaptiveTrigger), new PropertyMetadata(double.NaN));

		public static readonly DependencyProperty MinWindowHeightProperty = DependencyProperty.Register("MinWindowHeight", typeof(double), typeof(AdaptiveTrigger), new PropertyMetadata(double.NaN));

		public double MinWindowWidth
		{
			get
			{
				return (double)GetValue(MinWindowWidthProperty);
			}
			set
			{
				SetValue(MinWindowWidthProperty, value);
			}
		}

		public double MinWindowHeight
		{
			get
			{
				return (double)GetValue(MinWindowHeightProperty);
			}
			set
			{
				SetValue(MinWindowHeightProperty, value);
			}
		}

		protected override void OnAttached()
		{
			if (!DesignerProperties.GetIsInDesignMode(AssociatedObject))
			{
				Window window = Window.GetWindow(AssociatedObject);
				if (window == null)
				{
					AssociatedObject.Loaded += OnLoaded;
				}
				else if (window.ActualWidth <= 0.0 && window.ActualHeight <= 0.0)
				{
					AssociatedObject.Loaded += OnLoaded;
				}
				else
				{
					StartWatch();
				}
			}
		}

		protected override void OnDetaching()
		{
			if (!DesignerProperties.GetIsInDesignMode(AssociatedObject))
			{
				AssociatedObject.Loaded -= OnLoaded;
				Window.GetWindow(AssociatedObject).SizeChanged += OnWindowSizeChanged;
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			AssociatedObject.Loaded -= OnLoaded;
			StartWatch();
		}

		private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
		{
			Adapt(e.NewSize);
		}

		private void StartWatch()
		{
			Window window = Window.GetWindow(AssociatedObject);
			window.SizeChanged += OnWindowSizeChanged;
			if (AssociatedObject.IsLoaded)
			{
				Adapt(new Size(window.ActualWidth, window.ActualHeight));
				return;
			}
			Dispatcher.BeginInvoke((Action)(() =>
			{
				Adapt(new Size(window.ActualWidth, window.ActualHeight));
			}));
		}

		private void Adapt(Size size)
		{
			bool flag = false;
			if (!double.IsNaN(MinWindowWidth))
			{
				flag = size.Width >= MinWindowWidth;
			}
			if (!flag && !double.IsNaN(MinWindowHeight))
			{
				flag = size.Height >= MinWindowHeight;
			}
			if (flag)
			{
				InvokeActions(null);
			}
		}
	}
}
