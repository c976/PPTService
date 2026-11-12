using System;
using System.Windows;
using Cvte.Windows.Mathematics;

namespace Cvte.Internal.Ink
{
	internal static class DoubleUtil
	{
		private const double DblEpsilon = 2.22044604925031E-16;

		public static bool LessThan(double value1, double value2)
		{
			if (value1 < value2)
			{
				return !Nearly.Equals(value1, value2);
			}
			return false;
		}

		public static bool GreaterThan(double value1, double value2)
		{
			if (value1 > value2)
			{
				return !Nearly.Equals(value1, value2);
			}
			return false;
		}

		public static bool LessThanOrClose(double value1, double value2, double tolerance = 1E-05)
		{
			if (value1 >= value2)
			{
				return Nearly.Equals(value1, value2, tolerance);
			}
			return true;
		}

		public static bool GreaterThanOrClose(double value1, double value2)
		{
			if (value1 <= value2)
			{
				return Nearly.Equals(value1, value2);
			}
			return true;
		}

		public static bool IsOne(double value)
		{
			return Math.Abs(value - 1.0) < 2.22044604925031E-16;
		}

		public static bool IsZero(double value)
		{
			return Math.Abs(value) < 2.22044604925031E-16;
		}

		public static bool AreClose(Point point1, Point point2)
		{
			if (Nearly.Equals(point1.X, point2.X))
			{
				return Nearly.Equals(point1.Y, point2.Y);
			}
			return false;
		}

		public static bool AreClose(Vector vector1, Vector vector2)
		{
			if (Nearly.Equals(vector1.X, vector2.X))
			{
				return Nearly.Equals(vector1.Y, vector2.Y);
			}
			return false;
		}

		public static bool IsBetweenZeroAndOne(double val)
		{
			if (GreaterThanOrClose(val, 0.0))
			{
				return LessThanOrClose(val, 1.0);
			}
			return false;
		}
	}
}
