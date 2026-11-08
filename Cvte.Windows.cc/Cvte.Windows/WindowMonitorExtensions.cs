using System.Windows;
using System.Windows.Media;

namespace Cvte.Windows
{
	public static class WindowMonitorExtensions
	{
		public static MonitorDevice GetMonitor(this Visual visual)
		{
			return Window.GetWindow(visual).GetMonitor();
		}

		public static MonitorDevice GetMonitor(this Window window)
		{
			return new WindowMonitorHelper(window).Monitor;
		}

		public static bool MoveToNextMonitor(this Window window)
		{
			return new WindowMonitorHelper(window).MoveToNextMonitor();
		}

		public static void MoveToMonitor(this Window window, MonitorDevice monitor)
		{
			new WindowMonitorHelper(window).MoveToMonitor(monitor);
		}

		public static void ShowOnMonitor(this Window window, MonitorDevice monitor)
		{
		}

		public static void ShowDialogOnMonitor(this Window window, MonitorDevice monitor)
		{
		}
	}
}
