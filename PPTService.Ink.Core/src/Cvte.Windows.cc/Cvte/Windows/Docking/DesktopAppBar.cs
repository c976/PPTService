using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows.Docking
{
	public class DesktopAppBar
	{
		private class AppBarWindowProcessor
		{
			private readonly Window _window;

			private readonly TaskCompletionSource<HwndSource> _hwndSourceTask;

			private readonly int _callbackId;

			private WindowStyle _restoreStyle;

			private Rect _restoreBounds;

			private ResizeMode _restoreResizeMode;

			private bool _restoreTopmost;

			private AppBarEdge Edge { get; set; }

			public AppBarWindowProcessor(Window window)
			{
				_window = window;
				_callbackId = Win32.User32.RegisterWindowMessage("AppBarMessage");
				_hwndSourceTask = new TaskCompletionSource<HwndSource>();
				HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(window);
				if (hwndSource == null)
				{
					window.SourceInitialized += OnSourceInitialized;
				}
				else
				{
					_hwndSourceTask.SetResult(hwndSource);
				}
				_window.Closed += OnClosed;
			}

			private void OnSourceInitialized(object sender, EventArgs e)
			{
				_window.SourceInitialized -= OnSourceInitialized;
				HwndSource result = (HwndSource)PresentationSource.FromVisual(_window);
				_hwndSourceTask.SetResult(result);
			}

			private void OnClosed(object sender, EventArgs e)
			{
				_window.Closed -= OnClosed;
				_window.ClearValue(AppBarProperty);
			}

			private void ForceWindowProperties()
			{
				_window.WindowStyle = WindowStyle.None;
				_window.ResizeMode = ResizeMode.NoResize;
				_window.Topmost = true;
			}

			private void BackupWindowProperties()
			{
				_restoreStyle = _window.WindowStyle;
				_restoreBounds = _window.RestoreBounds;
				_restoreResizeMode = _window.ResizeMode;
				_restoreTopmost = _window.Topmost;
			}

			public async void Attach(AppBarEdge value)
			{
				HwndSource hwndSource = await _hwndSourceTask.Task;
				BackupWindowProperties();
				Win32.Shell32.APPBARDATA pData = default(Win32.Shell32.APPBARDATA);
				pData.cbSize = Marshal.SizeOf((object)pData);
				pData.hWnd = hwndSource.Handle;
				pData.uCallbackMessage = _callbackId;
				Win32.Shell32.SHAppBarMessage(0, ref pData);
				hwndSource.AddHook(WndProc);
				Update(value);
			}

			public async void Update(AppBarEdge value)
			{
				HwndSource hwndSource = await _hwndSourceTask.Task;
				Edge = value;
				Rect bounds = TransformToAppBar(hwndSource.Handle, _window.RestoreBounds, value);
				ForceWindowProperties();
				Resize(_window, bounds);
			}

			public async void Detach()
			{
				HwndSource hwndSource = await _hwndSourceTask.Task;
				Win32.Shell32.APPBARDATA pData = default(Win32.Shell32.APPBARDATA);
				pData.cbSize = Marshal.SizeOf((object)pData);
				pData.hWnd = hwndSource.Handle;
				Win32.Shell32.SHAppBarMessage(1, ref pData);
				_window.WindowStyle = _restoreStyle;
				_window.ResizeMode = _restoreResizeMode;
				_window.Topmost = _restoreTopmost;
				Resize(_window, _restoreBounds);
			}

			private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
			{
				if (msg == _callbackId && wParam.ToInt32() == 1)
				{
					HwndSource result = _hwndSourceTask.Task.Result;
					Rect bounds = TransformToAppBar(result.Handle, _window.RestoreBounds, Edge);
					Resize(_window, bounds);
					handled = true;
				}
				return IntPtr.Zero;
			}

			private static void Resize(Window window, Rect bounds)
			{
				window.Left = bounds.Left;
				window.Top = bounds.Top;
				window.Width = bounds.Width;
				window.Height = bounds.Height;
			}

			private Rect TransformToAppBar(IntPtr hWnd, Rect area, AppBarEdge edge)
			{
				Win32.Shell32.APPBARDATA pData = default(Win32.Shell32.APPBARDATA);
				pData.cbSize = Marshal.SizeOf((object)pData);
				pData.hWnd = hWnd;
				pData.uEdge = (int)edge;
				if (pData.uEdge == 0 || pData.uEdge == 2)
				{
					pData.rc.Top = 0;
					pData.rc.Bottom = (int)SystemParameters.PrimaryScreenHeight;
					if (pData.uEdge == 0)
					{
						pData.rc.Left = 0;
						pData.rc.Right = (int)Math.Round(area.Width);
					}
					else
					{
						pData.rc.Right = (int)SystemParameters.PrimaryScreenWidth;
						pData.rc.Left = pData.rc.Right - (int)Math.Round(area.Width);
					}
				}
				else
				{
					pData.rc.Left = 0;
					pData.rc.Right = (int)SystemParameters.PrimaryScreenWidth;
					if (pData.uEdge == 1)
					{
						pData.rc.Top = 0;
						pData.rc.Bottom = (int)Math.Round(area.Height);
					}
					else
					{
						pData.rc.Bottom = (int)SystemParameters.PrimaryScreenHeight;
						pData.rc.Top = pData.rc.Bottom - (int)Math.Round(area.Height);
					}
				}
				Win32.Shell32.SHAppBarMessage(2, ref pData);
				Win32.Shell32.SHAppBarMessage(3, ref pData);
				return new Rect(pData.rc.Left, pData.rc.Top, pData.rc.Right - pData.rc.Left, pData.rc.Bottom - pData.rc.Top);
			}
		}

		public static readonly DependencyProperty AppBarProperty = DependencyProperty.RegisterAttached("AppBar", typeof(AppBarEdge), typeof(DesktopAppBar), new PropertyMetadata(AppBarEdge.None, OnAppBarEdgeChanged));

		private static readonly DependencyProperty AppBarProcessorProperty = DependencyProperty.RegisterAttached("AppBarProcessor", typeof(AppBarWindowProcessor), typeof(DesktopAppBar), new PropertyMetadata(null));

		public static AppBarEdge GetAppBar(Window window)
		{
			return (AppBarEdge)window.GetValue(AppBarProperty);
		}

		public static void SetAppBar(Window window, AppBarEdge value)
		{
			window.SetValue(AppBarProperty, value);
		}

		private static void OnAppBarEdgeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(d))
			{
				AppBarEdge appBarEdge = (AppBarEdge)e.OldValue;
				AppBarEdge appBarEdge2 = (AppBarEdge)e.NewValue;
				bool flag = appBarEdge == AppBarEdge.Left || appBarEdge == AppBarEdge.Top || appBarEdge == AppBarEdge.Right || appBarEdge == AppBarEdge.Bottom;
				bool flag2 = appBarEdge2 == AppBarEdge.Left || appBarEdge2 == AppBarEdge.Top || appBarEdge2 == AppBarEdge.Right || appBarEdge2 == AppBarEdge.Bottom;
				if (flag && !flag2)
				{
					((AppBarWindowProcessor)d.GetValue(AppBarProcessorProperty)).Detach();
				}
				else if (!flag & flag2)
				{
					AppBarWindowProcessor appBarWindowProcessor = new AppBarWindowProcessor((Window)d);
					d.SetValue(AppBarProcessorProperty, appBarWindowProcessor);
					appBarWindowProcessor.Attach(appBarEdge2);
				}
				else if (flag & flag2)
				{
					((AppBarWindowProcessor)d.GetValue(AppBarProcessorProperty)).Update(appBarEdge2);
				}
			}
		}
	}
}
