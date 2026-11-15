namespace PPTService.Ink.Core.Windows.Native
{
	internal static class IntExtensions
	{
		public static short Low(this int dword)
		{
			return (short)dword;
		}

		public static int WithLow(this int dword, short low16)
		{
			return (int)(dword & 0xFFFF0000u) | (ushort)low16;
		}

		public static short High(this int dword)
		{
			return (short)(dword >> 16);
		}

		public static int WithHigh(this int dword, short high16)
		{
			return (high16 << 16) | dword.LowAsInt();
		}

		public static int LowAsInt(this int dword)
		{
			return dword & 0xFFFF;
		}

		public static int HighAsInt(this int dword)
		{
			return (dword >> 16) & 0xFFFF;
		}
	}
}
