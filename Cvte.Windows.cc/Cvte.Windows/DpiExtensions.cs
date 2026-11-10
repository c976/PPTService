using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cvte.Windows.Win32;

namespace Cvte.Windows
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
			return (PresentationSource.FromVisual(visual) as HwndSource)?.GetMonitorDpi() ?? Dpi.System;
		}

		internal static Dpi GetMonitorDpi(this HwndSource hwndSource)
		{
			if (!Monitor.IsPerMonitorDpiSupported)
			{
				return Dpi.System;
			}
			IntPtr hmonitor = NativeMethods.MonitorFromWindow(hwndSource.Handle, MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);
			return GetMonitorDpi(hmonitor);
		}

		internal unsafe static Dpi GetMonitorDpi(IntPtr hmonitor)
		{
			void* ptr = stackalloc byte[8];
			if (!Monitor.IsPerMonitorDpiSupported)
			{
				return Dpi.System;
			}
			*(int*)ptr = 1;
			((int*)ptr)[1] = 1;
			NativeMethods.GetDpiForMonitor(hmonitor, MonitorDpiType.EffectiveDpi, ref *(uint*)ptr, ref *(uint*)((byte*)ptr + 4));
			return new Dpi(*(int*)ptr, ((int*)ptr)[1]);
		}
	}
}
