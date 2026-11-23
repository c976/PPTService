namespace PPTServiceUltra.Ink.Core.Windows.Native
{
	internal static class LongExtensions
	{
		public static int Low(this long qWord)
		{
			return (int)qWord;
		}

		public static long WithLow(this long qWord, int low32)
		{
			return (qWord & -4294967296L) | (uint)low32;
		}

		public static int High(this long qWord)
		{
			return (int)(qWord >> 32);
		}

		public static long WithHigh(this long qWord, int high32)
		{
			return ((long)high32 << 32) | qWord.LowAsLong();
		}

		public static long LowAsLong(this long qWord)
		{
			return qWord & 0xFFFFFFFFu;
		}

		public static long HighAsLong(this long qWord)
		{
			return (qWord >> 32) & 0xFFFFFFFFu;
		}
	}
}
