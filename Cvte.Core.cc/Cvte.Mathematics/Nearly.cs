using System;
using System.Windows;

namespace Cvte.Mathematics
{
	public static class Nearly
	{
		internal const double AlmostZero = 1E-05;

		public static bool Equals(Point p1, Point p2, double tolerance = 1E-05)
		{
			if (Equals(p1.X, p2.X, tolerance))
			{
				return Equals(p1.Y, p2.Y, tolerance);
			}
			return false;
		}

		public static bool Equals(Line line1, Line line2, double tolerance = 1E-05)
		{
			if (Equals(line1.A, line2.A, tolerance) && Equals(line1.B, line2.B, tolerance))
			{
				return Equals(line1.C, line2.C, tolerance);
			}
			return false;
		}

		public static bool Equals(Vector v1, Vector v2, double tolerance = 1E-05)
		{
			if (Equals(v1.X, v2.X, tolerance))
			{
				return Equals(v1.Y, v2.Y, tolerance);
			}
			return false;
		}

		public static bool Equals(Rect r1, Rect r2, double tolerance = 1E-05)
		{
			if (Equals(r1.Width, r2.Width, tolerance) && Equals(r1.Height, r2.Height, tolerance) && Equals(r1.X, r2.X, tolerance))
			{
				return Equals(r1.Y, r2.Y, tolerance);
			}
			return false;
		}

		public static bool Equals(double d1, double d2, double tolerance = 1E-05)
		{
			return Math.Abs(d1 - d2) < tolerance;
		}

		public static bool Equals(Size size1, Size size2, double tolerance = 1E-05)
		{
			if (Equals(size1.Width, size2.Width, tolerance))
			{
				return Equals(size1.Height, size2.Height, tolerance);
			}
			return false;
		}

		[Obsolete("Nearly doesn't support comparision of these types.", true)]
		public new static bool Equals(object obj1, object obj2)
		{
			throw new NotSupportedException($"Nearly doesn't support comparision between {obj1?.GetType()} and {obj2?.GetType()}");
		}
	}
}
