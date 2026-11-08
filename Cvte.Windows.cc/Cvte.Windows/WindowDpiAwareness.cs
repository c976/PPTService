using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cvte.Windows.Win32;

namespace Cvte.Windows
{
	public sealed class WindowDpiAwareness : DependencyObject
	{
		public static readonly DependencyProperty WindowDpiAwarenessProperty;

		private static readonly DependencyProperty MonitorDpiOfWindowProperty;

		public static readonly DpiChangedEventHandler VisualParentScaleTransformForDpiChanged;

		public static readonly DependencyProperty DpiAwarenessProperty;

		private DpiChangedEventHandler m_DpiChanged;

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

		public event DpiChangedEventHandler DpiChanged
		{
			add
			{
				DpiChangedEventHandler dpiChangedEventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						dpiChangedEventHandler = this.m_DpiChanged;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				DpiChangedEventHandler dpiChangedEventHandler2 = default(DpiChangedEventHandler);
				DpiChangedEventHandler value2 = (DpiChangedEventHandler)Delegate.Combine(dpiChangedEventHandler2, value);
				dpiChangedEventHandler = Interlocked.CompareExchange(ref this.m_DpiChanged, value2, dpiChangedEventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				DpiChangedEventHandler dpiChangedEventHandler3 = dpiChangedEventHandler;
				if (6u != 0)
				{
					dpiChangedEventHandler2 = dpiChangedEventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)dpiChangedEventHandler != dpiChangedEventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			remove
			{
				DpiChangedEventHandler dpiChangedEventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						dpiChangedEventHandler = this.m_DpiChanged;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				DpiChangedEventHandler dpiChangedEventHandler2 = default(DpiChangedEventHandler);
				DpiChangedEventHandler value2 = (DpiChangedEventHandler)Delegate.Remove(dpiChangedEventHandler2, value);
				dpiChangedEventHandler = Interlocked.CompareExchange(ref this.m_DpiChanged, value2, dpiChangedEventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				DpiChangedEventHandler dpiChangedEventHandler3 = dpiChangedEventHandler;
				if (6u != 0)
				{
					dpiChangedEventHandler2 = dpiChangedEventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)dpiChangedEventHandler != dpiChangedEventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

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
				goto IL_0079;
			}
			WindowDpiAwareness windowDpiAwareness = (WindowDpiAwareness)args.OldValue;
			if (windowDpiAwareness != null)
			{
				goto IL_003a;
			}
			goto IL_0043;
			IL_0053:
			if (false)
			{
				goto IL_0079;
			}
			if (false)
			{
				goto IL_003a;
			}
			WindowDpiAwareness windowDpiAwareness2 = default(WindowDpiAwareness);
			windowDpiAwareness2?.OnAttached(window);
			if (0 == 0)
			{
				return;
			}
			goto IL_0043;
			IL_003a:
			if (6 != 0)
			{
				windowDpiAwareness.OnDetaching();
				goto IL_0043;
			}
			goto IL_0053;
			IL_0079:
			if (dp.IsDesignTimeWindow())
			{
				return;
			}
			throw new InvalidOperationException("WindowDpiAwareness 属性只能附加到 Window 上。");
			IL_0043:
			if (false)
			{
				return;
			}
			windowDpiAwareness2 = (WindowDpiAwareness)args.NewValue;
			goto IL_0053;
		}

		private void HandleAndRaiseDpiChanged(Dpi dpi)
		{
			do
			{
				IL_005a:
				DpiChangedEventHandler dpiChanged = this.m_DpiChanged;
				if (dpiChanged == null)
				{
					continue;
				}
				Dpi monitorDpiOfWindow = GetMonitorDpiOfWindow(_target);
				if (monitorDpiOfWindow != dpi)
				{
					Dpi systemDpi = _target.GetSystemDpi();
					if (false)
					{
						goto IL_005a;
					}
					SetMonitorDpiOfWindow(_target, dpi);
					dpiChanged(this, new DpiChangedEventArgs(_target, monitorDpiOfWindow, dpi, systemDpi));
				}
			}
			while (false);
		}

		private void OnAttached(Window target)
		{
			_target = target;
			do
			{
				if (target.IsLoaded)
				{
					HandleTargetDpi();
					continue;
				}
				target.SourceInitialized += TargetWindow_SourceInitialized;
				if (8 != 0)
				{
					break;
				}
			}
			while (false);
		}

		private void OnDetaching()
		{
			HwndSource hwndSource;
			while (true)
			{
				hwndSource = PresentationSource.FromVisual(_target) as HwndSource;
				if (3 != 0)
				{
					if (6 == 0)
					{
						return;
					}
					if (hwndSource != null)
					{
						break;
					}
				}
				if (7 == 0)
				{
					continue;
				}
				if (false)
				{
				}
				return;
			}
			if (Monitor.IsPerMonitorDpiSupported)
			{
				hwndSource.RemoveHook(WndProc);
			}
			_target = null;
		}

		private void TargetWindow_SourceInitialized(object sender, EventArgs e)
		{
			if (false || _target != null)
			{
				goto IL_002d;
			}
			goto IL_003a;
			IL_003a:
			HandleTargetDpi();
			if (false)
			{
				goto IL_002d;
			}
			return;
			IL_002d:
			_target.SourceInitialized -= TargetWindow_SourceInitialized;
			goto IL_003a;
		}

		private void HandleTargetDpi()
		{
			HwndSource hwndSource = PresentationSource.FromVisual(_target) as HwndSource;
			while (hwndSource != null)
			{
				while (true)
				{
					Dpi dpi;
					if (8 != 0)
					{
						if (false)
						{
							break;
						}
						Dpi systemDpi = _target.GetSystemDpi();
						if (Monitor.IsPerMonitorDpiSupported)
						{
							dpi = hwndSource.GetMonitorDpi();
							goto IL_0032;
						}
						dpi = systemDpi;
						goto IL_0048;
					}
					goto IL_0052;
					IL_0048:
					if (false)
					{
						continue;
					}
					HandleAndRaiseDpiChanged(dpi);
					goto IL_0052;
					IL_0052:
					if (0 == 0)
					{
						return;
					}
					goto IL_0032;
					IL_0032:
					hwndSource.AddHook(WndProc);
					goto IL_0048;
				}
			}
		}

		private unsafe IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			void* ptr = stackalloc byte[4];
			if (0 == 0)
			{
				if (-1 == 0)
				{
					goto IL_0050;
				}
			}
			if (msg != 736)
			{
				goto IL_0044;
			}
			goto IL_0050;
			IL_0050:
			if (DpiAwareness == DpiAwareness.PerMonitor)
			{
				*(ushort*)ptr = wParam.ToHiWord();
				((short*)ptr)[1] = (short)wParam.ToLoWord();
				HandleAndRaiseDpiChanged(new Dpi(*(ushort*)ptr, ((ushort*)ptr)[1]));
				handled = true;
			}
			goto IL_0044;
			IL_0044:
			return IntPtr.Zero;
		}

		unsafe static WindowDpiAwareness()
		{
			if (0 == 0)
			{
				WindowDpiAwarenessProperty = DependencyProperty.RegisterAttached("WindowDpiAwareness", typeof(WindowDpiAwareness), typeof(WindowDpiAwareness), new PropertyMetadata(null, WindowDpiAwarenessPropertyChangedCallback));
			}
			if (0 == 0)
			{
				MonitorDpiOfWindowProperty = DependencyProperty.RegisterAttached("MonitorDpiOfWindow", typeof(Dpi), typeof(WindowDpiAwareness), new PropertyMetadata(Dpi.ScreenStandard));
				VisualParentScaleTransformForDpiChanged = (object sender, DpiChangedEventArgs args) =>
				{
					int num = 16;
					void* ptr = stackalloc byte[num];
					if (num != 0)
					{
						num = (Monitor.IsPerMonitorDpiSupported ? 1 : 0);
					}
					if (num != 0)
					{
						PresentationSource presentationSource = PresentationSource.FromVisual(args.Window);
						PresentationSource presentationSource2;
						if (true)
						{
							presentationSource2 = presentationSource;
						}
						if (presentationSource2 != null)
						{
							FrameworkElement frameworkElement = VisualTreeHelper.GetChild(presentationSource2.RootVisual, 0) as FrameworkElement;
							if (frameworkElement != null)
							{
								Rect restoreBounds = default(Rect);
								Rect restoreBounds2 = default(Rect);
								if (8 != 0)
								{
									frameworkElement.LayoutTransform = ((args.NewMonitorDpi == args.SystemDpi) ? Transform.Identity : new ScaleTransform((double)args.NewMonitorDpi.X / (double)args.SystemDpi.X, (double)args.NewMonitorDpi.Y / (double)args.SystemDpi.Y));
									restoreBounds = args.Window.RestoreBounds;
									*(double*)ptr = args.Window.RestoreBounds.Width * (double)args.NewMonitorDpi.X / (double)args.OldMonitorDpi.X;
									restoreBounds2 = args.Window.RestoreBounds;
								}
								((double*)ptr)[1] = restoreBounds2.Height * (double)args.NewMonitorDpi.Y / (double)args.OldMonitorDpi.Y;
								args.Window.Left = restoreBounds.Left - (*(double*)ptr - restoreBounds.Width) / 2.0;
								args.Window.Width = *(double*)ptr;
								args.Window.Height = ((double*)ptr)[1];
							}
						}
					}
				};
			}
			DpiAwarenessProperty = DependencyProperty.Register("DpiAwareness", typeof(DpiAwareness), typeof(WindowDpiAwareness), new PropertyMetadata(DpiAwareness.System));
		}
	}
}
