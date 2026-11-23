using System;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class CoerceCorrect
	{
		public static int CoerceValue(this int value, int minimum, int maximum)
		{
			return Math.Max(Math.Min(value, maximum), minimum);
		}

		public static double CoerceValue(this double value, double minimum, double maximum)
		{
			return Math.Max(Math.Min(value, maximum), minimum);
		}
	}
}
