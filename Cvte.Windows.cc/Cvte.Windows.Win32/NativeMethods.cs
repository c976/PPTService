using System;
using System.Runtime.InteropServices;

namespace Cvte.Windows.Win32
{
	internal class NativeMethods
	{
		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorDefaultTo dwFlags);

		[DllImport("SHCore.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
		internal static extern void GetDpiForMonitor(IntPtr hmonitor, MonitorDpiType dpiType, ref uint dpiX, ref uint dpiY);
	}
}
