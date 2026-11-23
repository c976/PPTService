using System;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.Native
{
	internal class NativeMethods
	{
		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[Obsolete("请使用 Win32.User32.MonitorFromWindow")]
		internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorDefaultTo dwFlags);

		[DllImport("SHCore.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
		[Obsolete("请使用 Win32.ShCore.GetDpiForMonitor")]
		internal static extern void GetDpiForMonitor(IntPtr hmonitor, MonitorDpiType dpiType, ref uint dpiX, ref uint dpiY);
	}
}
