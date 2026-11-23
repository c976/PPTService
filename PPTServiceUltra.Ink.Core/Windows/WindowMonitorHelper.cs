using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class WindowMonitorHelper
	{
		private readonly Visual _visual;

		private static readonly Func<Rect, Point> CenterOf = (Rect bounds) => new Point(bounds.Left + bounds.Width / 2.0, bounds.Top + bounds.Height / 2.0);

		public MonitorDevice Monitor
		{
			get
			{
				HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(_visual);
				if (hwndSource == null)
				{
					return null;
				}
				return GetMonitorDeviceFromWindow(hwndSource.Handle);
			}
		}

		public WindowMonitorHelper(Visual visual)
		{
			_visual = visual;
		}

		public bool MoveToNextMonitor()
		{
			MonitorDevice current = Monitor;
			MonitorDevice monitorDevice = PPTServiceUltra.Ink.Core.Windows.Monitor.Devices.FirstOrDefault((MonitorDevice x) => !object.Equals(x, current));
			if (monitorDevice == null)
			{
				return false;
			}
			MoveToMonitor(monitorDevice);
			return true;
		}

		public void MoveToMonitor(MonitorDevice monitor)
		{
			MonitorDevice monitor2 = Monitor;
			if (!object.Equals(monitor2, monitor))
			{
				Window window = _visual as Window;
				if (window != null)
				{
					Size size = window.RestoreBounds.Size;
					Rect pixelBounds = monitor2.PixelBounds;
					Rect pixelBounds2 = monitor.PixelBounds;
					Rect restoreBounds = window.RestoreBounds;
					double num = (pixelBounds2.Width - size.Width) / (pixelBounds.Width - size.Width);
					double num2 = (pixelBounds2.Height - size.Height) / (pixelBounds.Height - size.Height);
					Point point = CenterOf(restoreBounds);
					Point point2 = new Point(point.X - pixelBounds.Left - size.Width / 2.0, point.Y - pixelBounds.Top - size.Height / 2.0);
					Point point3 = new Point(point2.X * num + size.Width / 2.0, point2.Y * num2 + size.Height / 2.0);
					Rect rect = new Rect(pixelBounds2.Left + point3.X - size.Width / 2.0, pixelBounds2.Top + point3.Y - size.Height / 2.0, restoreBounds.Width, restoreBounds.Height);
					window.Left = rect.Left;
					window.Top = rect.Top;
				}
			}
		}

		internal static MonitorDevice GetMonitorDeviceFromWindow(IntPtr windowHandle)
		{
			if (windowHandle == IntPtr.Zero)
			{
				return null;
			}
			return PPTServiceUltra.Ink.Core.Windows.Monitor.FromWin32Screen(Screen.FromHandle(windowHandle));
		}
	}
}
