using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	internal static class Geometry2DHelper
	{
		public static bool ContainsInCircles(Point point, Point[] 圆心坐标列表, double 圆角半径)
		{
			double num = 圆角半径 * 圆角半径;
			for (int i = 0; i < 圆心坐标列表.Length; i++)
			{
				Point point2 = 圆心坐标列表[i];
				if ((point.X - point2.X) * (point.X - point2.X) + (point.Y - point2.Y) * (point.Y - point2.Y) <= num)
				{
					return true;
				}
			}
			return false;
		}

		public static bool ContainsInPolygon(Point point, Point[] polygon)
		{
			for (int i = 1; i < polygon.Length; i++)
			{
				if (DoCrossProduct(point, polygon[i], polygon[i - 1]) > 0.0)
				{
					return false;
				}
			}
			return true;
		}

		private static double DoCrossProduct(Point point, Point currentPoint, Point anticlockwiseNextPoint)
		{
			return (currentPoint.X - point.X) * (anticlockwiseNextPoint.Y - point.Y) - (currentPoint.Y - point.Y) * (anticlockwiseNextPoint.X - point.X);
		}
	}
}
