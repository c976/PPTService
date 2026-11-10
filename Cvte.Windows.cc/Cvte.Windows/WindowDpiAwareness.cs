using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cvte.Windows.Win32;

namespace Cvte.Windows
{
	public sealed class WindowDpiAwareness : DependencyObject
	{
		public static readonly DependencyProperty WindowDpiAwarenessProperty = DependencyProperty.RegisterAttached("WindowDpiAwareness", typeof(WindowDpiAwareness), typeof(WindowDpiAwareness), new PropertyMetadata(null, WindowDpiAwarenessPropertyChangedCallback));

		private static readonly DependencyProperty MonitorDpiOfWindowProperty = DependencyProperty.RegisterAttached("MonitorDpiOfWindow", typeof(Dpi), typeof(WindowDpiAwareness), new PropertyMetadata(Dpi.ScreenStandard));

		public unsafe static readonly DpiChangedEventHandler VisualParentScaleTransformForDpiChanged = (object sender, DpiChangedEventArgs e) =>
		{
			void* ptr = stackalloc byte[16];
			if (Monitor.IsPerMonitorDpiSupported)
			{
				PresentationSource presentationSource = PresentationSource.FromVisual(e.Window);
				if (presentationSource != null)
				{
					FrameworkElement frameworkElement = VisualTreeHelper.GetChild(presentationSource.RootVisual, 0) as FrameworkElement;
					if (frameworkElement != null)
					{
						frameworkElement.LayoutTransform = ((e.NewMonitorDpi == e.SystemDpi) ? Transform.Identity : new ScaleTransform((double)e.NewMonitorDpi.X / (double)e.SystemDpi.X, (double)e.NewMonitorDpi.Y / (double)e.SystemDpi.Y));
						Rect restoreBounds = e.Window.RestoreBounds;
						*(double*)ptr = e.Window.RestoreBounds.Width * (double)e.NewMonitorDpi.X / (double)e.OldMonitorDpi.X;
						((double*)ptr)[1] = e.Window.RestoreBounds.Height * (double)e.NewMonitorDpi.Y / (double)e.OldMonitorDpi.Y;
						e.Window.Left = restoreBounds.Left - (*(double*)ptr - restoreBounds.Width) / 2.0;
						e.Window.Width = *(double*)ptr;
						e.Window.Height = ((double*)ptr)[1];
					}
				}
			}
		};

		public static readonly DependencyProperty DpiAwarenessProperty = DependencyProperty.Register("DpiAwareness", typeof(DpiAwareness), typeof(WindowDpiAwareness), new PropertyMetadata(DpiAwareness.System));

		private Window _target;

		public DpiAwareness DpiAwareness
		{
			get
			{
				return (DpiAwareness)GetValue(DpiAwarenessProperty);
			}
			set
			{
				SetValue(DpiAwarenessProperty, value);
			}
		}

		public event DpiChangedEventHandler DpiChanged;

		public static WindowDpiAwareness GetWindowDpiAwareness(DependencyObject element)
		{
			return (WindowDpiAwareness)element.GetValue(WindowDpiAwarenessProperty);
		}

		public static void SetWindowDpiAwareness(DependencyObject element, WindowDpiAwareness value)
		{
			element.SetValue(WindowDpiAwarenessProperty, value);
		}

		private static Dpi GetMonitorDpiOfWindow(Window window)
		{
			return (Dpi)window.GetValue(MonitorDpiOfWindowProperty);
		}

		private static void SetMonitorDpiOfWindow(Window window, Dpi value)
		{
			window.SetValue(MonitorDpiOfWindowProperty, value);
		}

		private static void WindowDpiAwarenessPropertyChangedCallback(DependencyObject dp, DependencyPropertyChangedEventArgs args)
		{
			Window window = dp as Window;
			if (window == null)
			{
				if (!dp.IsDesignTimeWindow())
				{
					throw new InvalidOperationException("WindowDpiAwareness 属性只能附加到 Window 上。");
				}
			}
			else
			{
				((WindowDpiAwareness)args.OldValue)?.OnDetaching();
				((WindowDpiAwareness)args.NewValue)?.OnAttached(window);
			}
		}

		private void HandleAndRaiseDpiChanged(Dpi dpi)
		{
			DpiChangedEventHandler dpiChanged = DpiChanged;
			if (dpiChanged != null)
			{
				Dpi monitorDpiOfWindow = GetMonitorDpiOfWindow(_target);
				if (monitorDpiOfWindow != dpi)
				{
					Dpi systemDpi = _target.GetSystemDpi();
					SetMonitorDpiOfWindow(_target, dpi);
					dpiChanged(this, new DpiChangedEventArgs(_target, monitorDpiOfWindow, dpi, systemDpi));
				}
			}
		}

		private void OnAttached(Window target)
		{
			_target = target;
			if (target.IsLoaded)
			{
				HandleTargetDpi();
			}
			else
			{
				target.SourceInitialized += TargetWindow_SourceInitialized;
			}
		}

		private void OnDetaching()
		{
			HwndSource hwndSource = PresentationSource.FromVisual(_target) as HwndSource;
			if (hwndSource != null)
			{
				if (Monitor.IsPerMonitorDpiSupported)
				{
					hwndSource.RemoveHook(WndProc);
				}
				_target = null;
			}
		}

		private void TargetWindow_SourceInitialized(object sender, EventArgs e)
		{
			if (_target != null)
			{
				_target.SourceInitialized -= TargetWindow_SourceInitialized;
			}
			HandleTargetDpi();
		}

		private void HandleTargetDpi()
		{
			HwndSource hwndSource = PresentationSource.FromVisual(_target) as HwndSource;
			if (hwndSource != null)
			{
				Dpi systemDpi = _target.GetSystemDpi();
				Dpi dpi;
				if (Monitor.IsPerMonitorDpiSupported)
				{
					dpi = hwndSource.GetMonitorDpi();
					hwndSource.AddHook(WndProc);
				}
				else
				{
					dpi = systemDpi;
				}
				HandleAndRaiseDpiChanged(dpi);
			}
		}

		private unsafe IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			void* ptr = stackalloc byte[4];
			if (msg == 736 && DpiAwareness == DpiAwareness.PerMonitor)
			{
				*(ushort*)ptr = wParam.ToHiWord();
				((short*)ptr)[1] = (short)wParam.ToLoWord();
				HandleAndRaiseDpiChanged(new Dpi(*(ushort*)ptr, ((ushort*)ptr)[1]));
				handled = true;
			}
			return IntPtr.Zero;
		}
	}
}
