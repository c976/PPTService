using System.Windows;

namespace Cvte.Mathematics
{
	public static class Finite
	{
		public static bool IsFinite(double d)
		{
			if (!double.IsInfinity(d))
			{
				return !double.IsNaN(d);
			}
			return false;
		}

		public static bool IsFinite(Point point)
		{
			if (IsFinite(point.X))
			{
				return IsFinite(point.Y);
			}
			return false;
		}

		public static bool IsFinite(Rect rect)
		{
			if (IsFinite(rect.TopLeft))
			{
				return IsFinite(rect.BottomRight);
			}
			return false;
		}

		public static bool IsFinite(Size size)
		{
			if (IsFinite(size.Width))
			{
				return IsFinite(size.Height);
			}
			return false;
		}
	}
}
