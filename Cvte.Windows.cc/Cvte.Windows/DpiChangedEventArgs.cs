using System;
using System.Windows;

namespace Cvte.Windows
{
	public class DpiChangedEventArgs : EventArgs
	{
		public Window Window { get; private set; }

		public Dpi OldMonitorDpi { get; private set; }

		public Dpi NewMonitorDpi { get; private set; }

		public Dpi SystemDpi { get; private set; }

		public DpiChangedEventArgs(Window window, Dpi oldMonitorDpi, Dpi newMonitorDpi, Dpi systemDpi)
		{
			Window = window;
			OldMonitorDpi = oldMonitorDpi;
			NewMonitorDpi = newMonitorDpi;
			SystemDpi = systemDpi;
		}
	}
}
