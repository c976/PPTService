using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Cvte.Windows
{
	public class WindowMonitorHelper
	{
		private readonly Window _window;

		private static readonly Func<Rect, Point> CenterOf = (Rect bounds) => new Point(bounds.Left + bounds.Width / 2.0, bounds.Top + bounds.Height / 2.0);

		public MonitorDevice Monitor
		{
			get
			{
				WindowInteropHelper windowInteropHelper = new WindowInteropHelper(_window);
				if (windowInteropHelper.Handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("窗口呈现之前或关闭之后无法获取其所在的屏幕。");
				}
				Screen screen = Screen.FromHandle(windowInteropHelper.Handle);
				return Cvte.Windows.Monitor.FromWin32Screen(screen);
			}
		}

		public WindowMonitorHelper(Window window)
		{
			_window = window;
		}

		public bool MoveToNextMonitor()
		{
			MonitorDevice current = Monitor;
			MonitorDevice monitorDevice = Cvte.Windows.Monitor.Devices.FirstOrDefault((MonitorDevice x) => !object.Equals(x, current));
			if (monitorDevice == null)
			{
				return false;
			}
			MoveToMonitor(monitorDevice);
			return true;
		}

		public unsafe void MoveToMonitor(MonitorDevice monitor)
		{
			void* ptr = stackalloc byte[16];
			MonitorDevice monitor2 = Monitor;
			if (!object.Equals(monitor2, monitor))
			{
				Size size = _window.RestoreBounds.Size;
				Rect pixelBounds = monitor2.PixelBounds;
				Rect pixelBounds2 = monitor.PixelBounds;
				Rect restoreBounds = _window.RestoreBounds;
				*(double*)ptr = (pixelBounds2.Width - size.Width) / (pixelBounds.Width - size.Width);
				((double*)ptr)[1] = (pixelBounds2.Height - size.Height) / (pixelBounds.Height - size.Height);
				Point point = CenterOf(restoreBounds);
				Point point2 = new Point(point.X - pixelBounds.Left - size.Width / 2.0, point.Y - pixelBounds.Top - size.Height / 2.0);
				Point point3 = new Point(point2.X * *(double*)ptr + size.Width / 2.0, point2.Y * ((double*)ptr)[1] + size.Height / 2.0);
				Rect rect = new Rect(pixelBounds2.Left + point3.X - size.Width / 2.0, pixelBounds2.Top + point3.Y - size.Height / 2.0, restoreBounds.Width, restoreBounds.Height);
				_window.Left = rect.Left;
				_window.Top = rect.Top;
			}
		}
	}
}
