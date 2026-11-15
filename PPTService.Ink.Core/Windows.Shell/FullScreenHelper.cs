using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows.Shell
{
	public static class FullScreenHelper
	{
		private static readonly DependencyProperty BeforeFullScreenWindowPlacementProperty = DependencyProperty.RegisterAttached("BeforeFullScreenWindowPlacement", typeof(Win32.WindowPlacement?), typeof(Window));

		private static readonly DependencyProperty BeforeFullScreenWindowStyleProperty = DependencyProperty.RegisterAttached("BeforeFullScreenWindowStyle", typeof(Win32.WindowStyles?), typeof(Window));

		public static void StartFullScreen(Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window", "window 不能为 null");
			}
			if (window.GetValue(BeforeFullScreenWindowPlacementProperty) == null && window.GetValue(BeforeFullScreenWindowStyleProperty) == null)
			{
				IntPtr intPtr = new WindowInteropHelper(window).EnsureHandle();
				HwndSource hwndSource = HwndSource.FromHwnd(intPtr);
				Win32.WindowPlacement lpwndpl = default(Win32.WindowPlacement);
				lpwndpl.Size = (uint)Marshal.SizeOf((object)lpwndpl);
				Win32.User32.GetWindowPlacement(intPtr, ref lpwndpl);
				window.SetValue(BeforeFullScreenWindowPlacementProperty, lpwndpl);
				Win32.WindowStyles windowStyles = (Win32.WindowStyles)(int)Win32.User32.GetWindowLongPtr(intPtr, Win32.GetWindowLongFields.GWL_STYLE);
				window.SetValue(BeforeFullScreenWindowStyleProperty, windowStyles);
				windowStyles &= ~(Win32.WindowStyles.WS_MAXIMIZE | Win32.WindowStyles.WS_MAXIMIZEBOX | Win32.WindowStyles.WS_SIZEBOX);
				Win32.User32.SetWindowLongPtr(intPtr, Win32.GetWindowLongFields.GWL_STYLE, (IntPtr)(int)windowStyles);
				int pvAttribute = 1;
				Win32.Dwmapi.DwmSetWindowAttribute(intPtr, Win32.DWMWINDOWATTRIBUTE.DWMWA_TRANSITIONS_FORCEDISABLED, ref pvAttribute, 4u);
				hwndSource.AddHook(KeepFullScreenHook);
				Win32.Rectangle lpRect;
				if (Win32.User32.GetWindowRect(intPtr, out lpRect))
				{
					Win32.User32.SetWindowPos(intPtr, (IntPtr)0, lpRect.Left, lpRect.Top, lpRect.Width, lpRect.Height, 4);
				}
			}
		}

		public static void EndFullScreen(Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window", "window 不能为 null");
			}
			object value = window.GetValue(BeforeFullScreenWindowPlacementProperty);
			if (!(value is Win32.WindowPlacement))
			{
				return;
			}
			Win32.WindowPlacement lpwndpl = (Win32.WindowPlacement)value;
			value = window.GetValue(BeforeFullScreenWindowStyleProperty);
			if (!(value is Win32.WindowStyles))
			{
				return;
			}
			Win32.WindowStyles windowStyles = (Win32.WindowStyles)value;
			IntPtr handle = new WindowInteropHelper(window).Handle;
			if (!(handle == IntPtr.Zero))
			{
				HwndSource hwndSource = HwndSource.FromHwnd(handle);
				hwndSource.RemoveHook(KeepFullScreenHook);
				Win32.User32.SetWindowLongPtr(handle, Win32.GetWindowLongFields.GWL_STYLE, (IntPtr)(int)(windowStyles & ~(Win32.WindowStyles.WS_ICONIC | Win32.WindowStyles.WS_MAXIMIZE)));
				if ((windowStyles & Win32.WindowStyles.WS_ICONIC) != 0)
				{
					lpwndpl.ShowCmd = Win32.ShowWindowCommands.SW_RESTORE;
				}
				if ((windowStyles & Win32.WindowStyles.WS_MAXIMIZE) != 0)
				{
					Win32.User32.ShowWindow(handle, Win32.ShowWindowCommands.SW_MAXIMIZE);
				}
				Win32.User32.SetWindowPlacement(handle, ref lpwndpl);
				Win32.Rectangle lpRect;
				if ((windowStyles & Win32.WindowStyles.WS_MAXIMIZE) == 0 && Win32.User32.GetWindowRect(handle, out lpRect))
				{
					Point point = hwndSource.CompositionTarget.TransformFromDevice.Transform(new Point(lpRect.Left, lpRect.Top));
					Point point2 = hwndSource.CompositionTarget.TransformFromDevice.Transform(new Point(lpRect.Width, lpRect.Height));
					window.Left = point.X;
					window.Top = point.Y;
					window.Width = point2.X;
					window.Height = point2.Y;
				}
				int pvAttribute = 0;
				Win32.Dwmapi.DwmSetWindowAttribute(handle, Win32.DWMWINDOWATTRIBUTE.DWMWA_TRANSITIONS_FORCEDISABLED, ref pvAttribute, 4u);
				window.ClearValue(BeforeFullScreenWindowPlacementProperty);
				window.ClearValue(BeforeFullScreenWindowStyleProperty);
			}
		}

		[HandleProcessCorruptedStateExceptions]
		private static IntPtr KeepFullScreenHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 70)
			{
				try
				{
					Win32.WindowPosition windowPosition = (Win32.WindowPosition)Marshal.PtrToStructure(lParam, typeof(Win32.WindowPosition));
					if ((windowPosition.Flags & Win32.WindowPositionFlags.SWP_NOMOVE) != 0 && (windowPosition.Flags & Win32.WindowPositionFlags.SWP_NOSIZE) != 0)
					{
						return IntPtr.Zero;
					}
					if (Win32.User32.IsIconic(hwnd))
					{
						return IntPtr.Zero;
					}
					Win32.Rectangle lpRect;
					if (Win32.User32.GetWindowRect(hwnd, out lpRect))
					{
						Win32.Rectangle lprc = lpRect;
						if ((windowPosition.Flags & Win32.WindowPositionFlags.SWP_NOMOVE) == 0)
						{
							lprc.Left = windowPosition.X;
							lprc.Top = windowPosition.Y;
						}
						if ((windowPosition.Flags & Win32.WindowPositionFlags.SWP_NOSIZE) == 0)
						{
							lprc.Right = lprc.Left + windowPosition.Width;
							lprc.Bottom = lprc.Top + windowPosition.Height;
						}
						else
						{
							lprc.Right = lprc.Left + lpRect.Width;
							lprc.Bottom = lprc.Top + lpRect.Height;
						}
						IntPtr hMonitor = Win32.User32.MonitorFromRect(ref lprc, Win32.MonitorFlag.MONITOR_DEFAULTTOPRIMARY);
						Win32.MonitorInfo lpmi = default(Win32.MonitorInfo);
						lpmi.Size = (uint)Marshal.SizeOf((object)lpmi);
						if (Win32.User32.GetMonitorInfo(hMonitor, ref lpmi))
						{
							windowPosition.X = lpmi.MonitorRect.Left;
							windowPosition.Y = lpmi.MonitorRect.Top;
							windowPosition.Width = lpmi.MonitorRect.Right - lpmi.MonitorRect.Left;
							windowPosition.Height = lpmi.MonitorRect.Bottom - lpmi.MonitorRect.Top;
							windowPosition.Flags &= ~(Win32.WindowPositionFlags.SWP_NOMOVE | Win32.WindowPositionFlags.SWP_NOREDRAW | Win32.WindowPositionFlags.SWP_NOSIZE);
							windowPosition.Flags |= Win32.WindowPositionFlags.SWP_NOCOPYBITS;
							if (lpRect == lpmi.MonitorRect)
							{
								HwndSource hwndSource = HwndSource.FromHwnd(hwnd);
								Window window = hwndSource?.RootVisual as Window;
								if (window != null)
								{
									Point point = hwndSource.CompositionTarget.TransformFromDevice.Transform(new Point(windowPosition.X, windowPosition.Y));
									Point point2 = hwndSource.CompositionTarget.TransformFromDevice.Transform(new Point(windowPosition.Width, windowPosition.Height));
									window.Left = point.X;
									window.Top = point.Y;
									window.Width = point2.X;
									window.Height = point2.Y;
								}
							}
							Marshal.StructureToPtr((object)windowPosition, lParam, false);
						}
					}
				}
				catch
				{
				}
			}
			return IntPtr.Zero;
		}
	}
}
