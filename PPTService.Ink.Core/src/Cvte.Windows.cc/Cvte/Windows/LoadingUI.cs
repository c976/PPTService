using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows
{
	public static class LoadingUI
	{
		public static Window AttachTo(IntPtr targetHwnd)
		{
			Window window = CreateInternal();
			window.SourceInitialized += (object sender, EventArgs e) =>
			{
				Window window2 = (Window)sender;
				if (Environment.OSVersion.Version >= new Version(6, 2))
				{
					SetLoadingWindowWithR(targetHwnd, window2.GetHandle());
				}
				else
				{
					window2.Topmost = true;
					SetLoadingWindow(targetHwnd, window2.GetHandle());
				}
			};
			return window;
		}

		private static void SetLoadingWindowWithR(IntPtr targetWindow, IntPtr loadingWindow)
		{
			Win32.SetParent(loadingWindow, targetWindow);
			Win32.SetWindowLong(loadingWindow, -16, 1342177280);
			Win32.RECT lpRect;
			Win32.GetWindowRect(targetWindow, out lpRect);
			int cx = lpRect.Right - lpRect.Left;
			int cy = lpRect.Bottom - lpRect.Top;
			Win32.SetWindowPos(loadingWindow, 0, 0, 0, 0, 0, 0);
			Win32.SetWindowPos(loadingWindow, 0, 0, 0, cx, cy, 0);
		}

		private static void SetLoadingWindow(IntPtr targetWindow, IntPtr loadingWindow)
		{
			Win32.RECT lpRect;
			Win32.GetWindowRect(targetWindow, out lpRect);
			int cx = lpRect.Right - lpRect.Left;
			int cy = lpRect.Bottom - lpRect.Top;
			Win32.SetWindowPos(loadingWindow, 0, lpRect.Left, lpRect.Top, cx, cy, 0);
		}

		private static Window CreateInternal()
		{
			Window window = new Window
			{
				WindowStyle = WindowStyle.None,
				WindowState = WindowState.Normal,
				AllowsTransparency = true,
				Background = Brushes.Transparent,
				ShowInTaskbar = false,
				Title = "LOADING",
				ShowActivated = false,
				Content = new Border
				{
					Background = new SolidColorBrush(Color.FromArgb(99, 0, 0, 0)),
					Child = new TextBlock
					{
						Text = "Loading"
					}
				}
			};
			WindowHelper.SetWindowNoActivate(window);
			return window;
		}
	}
}
