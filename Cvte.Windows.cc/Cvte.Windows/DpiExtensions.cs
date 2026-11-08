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
			if ((false || -1 == 0 || source != null) && source.CompositionTarget != null)
			{
				double num = Dpi.ScreenStandard.X;
				Matrix transformToDevice;
				if (0 == 0)
				{
					transformToDevice = source.CompositionTarget.TransformToDevice;
				}
				int x = (int)(num * transformToDevice.M11);
				double num2 = Dpi.ScreenStandard.Y;
				Matrix transformToDevice2 = default(Matrix);
				do
				{
					if (5 != 0)
					{
						transformToDevice2 = source.CompositionTarget.TransformToDevice;
					}
					num2 *= transformToDevice2.M22;
				}
				while (false);
				return new Dpi(x, (int)num2);
			}
			return Dpi.System;
		}

		public static Dpi GetMonitorDpi(this Visual visual)
		{
			HwndSource hwndSource = default(HwndSource);
			while (true)
			{
				hwndSource = PresentationSource.FromVisual(visual) as HwndSource;
				while (0 == 0)
				{
					if ((hwndSource != null) ? true : false)
					{
						goto end_IL_0000;
					}
					if (8 != 0)
					{
						return Dpi.System;
					}
				}
				continue;
				end_IL_0000:
				break;
			}
			return hwndSource.GetMonitorDpi();
		}

		internal static Dpi GetMonitorDpi(this HwndSource hwndSource)
		{
			do
			{
				if (false || !Monitor.IsPerMonitorDpiSupported)
				{
					return Dpi.System;
				}
			}
			while (false);
			IntPtr intPtr = hwndSource.Handle;
			do
			{
				intPtr = NativeMethods.MonitorFromWindow(intPtr, MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);
			}
			while (7 == 0 || 8 == 0);
			IntPtr hmonitor = intPtr;
			return GetMonitorDpi(hmonitor);
		}

		internal unsafe static Dpi GetMonitorDpi(IntPtr hmonitor)
		{
			int num = 8;
			while (true)
			{
				void* ptr = stackalloc byte[num];
				num = (Monitor.IsPerMonitorDpiSupported ? 1 : 0);
				while (7 != 0)
				{
					if (num != 0)
					{
						*(int*)ptr = 1;
						((int*)ptr)[1] = 1;
						NativeMethods.GetDpiForMonitor(hmonitor, MonitorDpiType.EffectiveDpi, ref *(uint*)ptr, ref *(uint*)((byte*)ptr + 4));
						num = *(int*)ptr;
						if (0 == 0)
						{
							return new Dpi(num, ((int*)ptr)[1]);
						}
						continue;
					}
					return Dpi.System;
				}
			}
		}
	}
}
