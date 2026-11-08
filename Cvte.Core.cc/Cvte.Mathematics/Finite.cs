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
			if (!IsFinite(point.X))
			{
				return false;
			}
			return IsFinite(point.Y);
		}

		public static bool IsFinite(Rect rect)
		{
			if (!IsFinite(rect.TopLeft))
			{
				return false;
			}
			return IsFinite(rect.BottomRight);
		}

		public static bool IsFinite(Size size)
		{
			if (!IsFinite(size.Width))
			{
				return false;
			}
			return IsFinite(size.Height);
		}
	}
}
