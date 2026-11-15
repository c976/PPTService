using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows
{
	public static class DpiExtensions
	{
		public static Dpi GetSystemDpi(this Visual visual)
		{
			return PresentationSource.FromVisual(visual)?.GetSystemDpi() ?? Dpi.System;
		}

		internal static Dpi GetSystemDpi(this PresentationSource source)
		{
			if (source != null && source.CompositionTarget != null)
			{
				return new Dpi((int)((double)Dpi.ScreenStandard.X * source.CompositionTarget.TransformToDevice.M11), (int)((double)Dpi.ScreenStandard.Y * source.CompositionTarget.TransformToDevice.M22));
			}
			return Dpi.System;
		}

		public static Dpi GetMonitorDpi(this Visual visual)
		{
			return (PresentationSource.FromVisual(visual) as HwndSource)?.GetMonitorDpi() ?? visual.GetSystemDpi();
		}

		internal static Dpi GetMonitorDpi(this HwndSource hwndSource)
		{
			if (!Monitor.IsPerMonitorDpiSupported)
			{
				return Dpi.System;
			}
			return GetMonitorDpi(NativeMethods.MonitorFromWindow(hwndSource.Handle, MonitorDefaultTo.MONITOR_DEFAULTTONEAREST));
		}

		internal static Dpi GetMonitorDpi(IntPtr hmonitor)
		{
			if (!Monitor.IsPerMonitorDpiSupported)
			{
				return Dpi.System;
			}
			uint dpiX = 1u;
			uint dpiY = 1u;
			try
			{
				NativeMethods.GetDpiForMonitor(hmonitor, MonitorDpiType.EffectiveDpi, ref dpiX, ref dpiY);
			}
			catch (COMException)
			{
				return Dpi.System;
			}
			return new Dpi((int)dpiX, (int)dpiY);
		}
	}
}
