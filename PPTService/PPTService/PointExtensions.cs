using System.Windows;

namespace PPTService
{
	public static class PointExtensions
	{
		public static Point OffsetEx(this Point point, double X, double Y)
		{
			return new Point(point.X + X, point.Y + Y);
		}
	}
}
