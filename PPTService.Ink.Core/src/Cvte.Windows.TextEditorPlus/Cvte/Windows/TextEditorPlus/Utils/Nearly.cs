using System;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class Nearly
	{
		internal const double AlmostZero = 1E-05;

		public static bool Equals(double d1, double d2, double tolerance = 1E-05)
		{
			return Math.Abs(d1 - d2) < tolerance;
		}
	}
}
