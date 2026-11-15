using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTService.Ink.Core.Mathematics
{
	[Obsolete("相关类型无法实现跨平台，因此请使用 Cvte.Windows 中的同名类型。")]
	public static class PointsCompute
	{
		public static Point SymmetricPointAboutPoint(Point basePoint, Point freePoint)
		{
			return new Point(2.0 * basePoint.X - freePoint.X, 2.0 * basePoint.Y - freePoint.Y);
		}

		public static Point SymmetricPointAboutLine(Point freePoint, LineSegment line)
		{
			double[] array = line.LineExpression();
			double num = array[0];
			double num2 = array[1];
			double num3 = array[2];
			double x = ((num2 * num2 - num * num) * freePoint.X - 2.0 * num * num2 * freePoint.Y - 2.0 * num * num3) / (num * num + num2 * num2);
			double y = ((num * num - num2 * num2) * freePoint.Y - 2.0 * num * num2 * freePoint.X - 2.0 * num2 * num3) / (num * num + num2 * num2);
			return new Point(x, y);
		}

		
		public static Point[] DistancePoint(Line line, Point freePoint, double distance)
		{
			return IntersectionCompute.LineAndCircle(line, freePoint, distance);
		}
	}
}
