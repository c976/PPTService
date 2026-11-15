using System;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Windows.Native
{
	internal static class Gdi
	{
		[DllImport("gdi32")]
		[Obsolete("请使用 Win32.Gdi32.DeleteObject")]
		internal static extern bool DeleteObject(IntPtr ptr);
	}
}
