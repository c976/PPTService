namespace PPTServiceUltra.Ink.Core.Windows.Native
{
	internal static class ULongExtensions
	{
		public static uint Low(this ulong qWord)
		{
			return (uint)qWord;
		}

		public static ulong WithLow(this ulong qWord, uint low32)
		{
			return (qWord & 0xFFFFFFFF00000000uL) | low32;
		}

		public static uint High(this ulong qWord)
		{
			return (uint)(qWord >> 32);
		}

		public static ulong WithHigh(this ulong qWord, uint high32)
		{
			return ((ulong)high32 << 32) | qWord.LowAsULong();
		}

		public static ulong LowAsULong(this ulong qWord)
		{
			return qWord & 0xFFFFFFFFu;
		}

		public static ulong HighAsULong(this ulong qWord)
		{
			return (qWord >> 32) & 0xFFFFFFFFu;
		}
	}
}
