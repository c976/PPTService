using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public static class TouchDragMoveWindowHelper
	{
		private class DragMoveMode
		{
			private readonly Window _window;

			private Win32.Point? _lastPoint;

			public DragMoveMode(Window window)
			{
				_window = window;
			}

			public void Start()
			{
				Window window = _window;
				window.PreviewMouseMove += Window_PreviewMouseMove;
				window.PreviewMouseUp += Window_PreviewMouseUp;
				window.LostMouseCapture += Window_LostMouseCapture;
			}

			public void Stop()
			{
				Window window = _window;
				window.PreviewMouseMove -= Window_PreviewMouseMove;
				window.PreviewMouseUp -= Window_PreviewMouseUp;
				window.LostMouseCapture -= Window_LostMouseCapture;
			}

			private void Window_LostMouseCapture(object sender, MouseEventArgs e)
			{
				Stop();
			}

			private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
			{
				Stop();
			}

			private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
			{
				Win32.Point point;
				Win32.User32.GetCursorPos(out point);
				if (!_lastPoint.HasValue)
				{
					_lastPoint = point;
					_window.CaptureMouse();
				}
				int num = point.X - _lastPoint.Value.X;
				int num2 = point.Y - _lastPoint.Value.Y;
				if (Math.Abs(num) < 60 && Math.Abs(num2) < 60)
				{
					IntPtr handle = new WindowInteropHelper(_window).Handle;
					Win32.Rectangle lpRect;
					Win32.User32.GetWindowRect(handle, out lpRect);
					Win32.User32.SetWindowPos(handle, IntPtr.Zero, lpRect.Left + num, lpRect.Top + num2, 0, 0, 5);
				}
				_lastPoint = point;
			}
		}

		public static void DragMove(Window window)
		{
			new DragMoveMode(window).Start();
		}
	}
}
