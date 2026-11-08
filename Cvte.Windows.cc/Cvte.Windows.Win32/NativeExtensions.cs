using System;

namespace Cvte.Windows.Win32
{
	internal static class NativeExtensions
	{
		internal static ushort ToLoWord(this IntPtr dword)
		{
			return (ushort)((int)dword & 0xFFFF);
		}

		internal static ushort ToHiWord(this IntPtr dword)
		{
			return (ushort)((uint)(int)dword >> 16);
		}
	}
}
