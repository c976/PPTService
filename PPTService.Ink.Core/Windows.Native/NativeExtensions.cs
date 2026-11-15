using System;

namespace PPTService.Ink.Core.Windows.Native
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

		public static int ToSafeInt32(this IntPtr ptr)
		{
			if (IntPtr.Size <= 4)
			{
				return ptr.ToInt32();
			}
			return (int)ptr.ToInt64();
		}

		public static uint ToSafeUInt32(this IntPtr ptr)
		{
			if (IntPtr.Size <= 4)
			{
				return (uint)ptr.ToInt32();
			}
			return (uint)ptr.ToInt64();
		}

		public static void BreakInt64Into32(this IntPtr ptr, out int high32, out int low32)
		{
			long qWord = ptr.ToInt64();
			low32 = qWord.Low();
			high32 = qWord.High();
		}

		public static void BreakInt64Into32(this IntPtr ptr, out long high32, out long low32)
		{
			long qWord = ptr.ToInt64();
			low32 = qWord.LowAsLong();
			high32 = qWord.HighAsLong();
		}

		public static void BreakInt64Into32Signed(this IntPtr ptr, out long high32, out long low32)
		{
			long qWord = ptr.ToInt64();
			low32 = qWord.Low();
			high32 = qWord.High();
		}

		public static void BreakInt32Into16(this IntPtr ptr, out int high16, out int low16)
		{
			int dword = ptr.ToInt32();
			low16 = dword.LowAsInt();
			high16 = dword.HighAsInt();
		}

		public static void BreakInt32Into16Signed(this IntPtr ptr, out int high16, out int low16)
		{
			int dword = ptr.ToInt32();
			low16 = dword.Low();
			high16 = dword.High();
		}

		public static void BreakInt32Into16(this IntPtr ptr, out short high16, out short low16)
		{
			int dword = ptr.ToInt32();
			low16 = dword.Low();
			high16 = dword.High();
		}

		public static void BreakSafeInt32To16(this IntPtr ptr, out short high16, out short low16)
		{
			int dword = ptr.ToSafeInt32();
			low16 = dword.Low();
			high16 = dword.High();
		}

		public static void BreakSafeInt32To16(this IntPtr ptr, out int high16, out int low16)
		{
			int dword = ptr.ToSafeInt32();
			low16 = dword.LowAsInt();
			high16 = dword.HighAsInt();
		}

		public static void BreakSafeInt32To16Signed(this IntPtr ptr, out int high16, out int low16)
		{
			int dword = ptr.ToSafeInt32();
			low16 = dword.Low();
			high16 = dword.High();
		}
	}
}
