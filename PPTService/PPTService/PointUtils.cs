using System;
using System.Windows;

namespace PPTService
{
	public static class PointUtils
	{
		public static Point ComputeCartesianCoordinate(double angle, double radius)
		{
			double num = Math.PI / 180.0 * (angle - 90.0);
			double x = radius * Math.Cos(num);
			double y = radius * Math.Sin(num);
			return new Point(x, y);
		}
	}
}
