namespace PPTServiceUltra.Ink.Core.Windows.Native
{
	internal static class UIntExtensions
	{
		public static ushort Low(this uint dword)
		{
			return (ushort)dword;
		}

		public static uint WithLow(this uint dword, ushort low16)
		{
			return (dword & 0xFFFF0000u) | low16;
		}

		public static ushort High(this uint dword)
		{
			return (ushort)(dword >> 16);
		}

		public static uint WithHigh(this uint dword, ushort high16)
		{
			return (uint)(high16 << 16) | dword.LowAsUInt();
		}

		public static uint LowAsUInt(this uint dword)
		{
			return dword & 0xFFFF;
		}

		public static uint HighAsUInt(this uint dword)
		{
			return (dword >> 16) & 0xFFFF;
		}
	}
}
