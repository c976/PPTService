using System;
using System.Runtime.InteropServices;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows.ScreenAdjuster
{
	internal class AdjustScreenByDxva2 : IAdjustScreen
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct PhysicalMonitor
		{
			public IntPtr hPhysicalMonitor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szPhysicalMonitorDescription;
		}

		[UsedImplicitly]
		internal AdjustScreenByDxva2()
		{
		}

		public bool SetBrightness(IntPtr handle, short brightness)
		{
			if (handle == IntPtr.Zero)
			{
				return false;
			}
			uint pdwNumberOfPhysicalMonitors = 0u;
			IntPtr hMonitor = Win32.User32.MonitorFromWindow(handle, Win32.MonitorFlag.MONITOR_DEFAULTTOPRIMARY);
			GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref pdwNumberOfPhysicalMonitors);
			PhysicalMonitor[] array = new PhysicalMonitor[pdwNumberOfPhysicalMonitors];
			GetPhysicalMonitorsFromHMONITOR(hMonitor, pdwNumberOfPhysicalMonitors, array);
			if (array.Length == 0)
			{
				return false;
			}
			return SetMonitorBrightness(array[0].hPhysicalMonitor, brightness);
		}

		public bool GetBrightness(IntPtr handle, ref short minBrightness, ref short currentBrightness, ref short maxBrightness)
		{
			if (handle == IntPtr.Zero)
			{
				return false;
			}
			uint pdwNumberOfPhysicalMonitors = 0u;
			IntPtr hMonitor = Win32.User32.MonitorFromWindow(handle, Win32.MonitorFlag.MONITOR_DEFAULTTOPRIMARY);
			GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref pdwNumberOfPhysicalMonitors);
			PhysicalMonitor[] array = new PhysicalMonitor[pdwNumberOfPhysicalMonitors];
			GetPhysicalMonitorsFromHMONITOR(hMonitor, pdwNumberOfPhysicalMonitors, array);
			if (array.Length == 0)
			{
				return false;
			}
			return GetMonitorBrightness(array[0].hPhysicalMonitor, ref minBrightness, ref currentBrightness, ref maxBrightness);
		}

		[DllImport("dxva2.dll")]
		private static extern bool SetMonitorBrightness(IntPtr hMonitor, short brightness);

		[DllImport("dxva2.dll")]
		private static extern bool GetMonitorBrightness(IntPtr hMonitor, ref short pdwMinimumBrightness, ref short pdwCurrentBrightness, ref short pdwMaximumBrightness);

		[DllImport("dxva2.dll")]
		private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

		[DllImport("dxva2.dll")]
		private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PhysicalMonitor[] pPhysicalMonitorArray);
	}
}
