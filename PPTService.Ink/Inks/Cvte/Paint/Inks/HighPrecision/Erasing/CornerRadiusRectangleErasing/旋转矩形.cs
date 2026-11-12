using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	internal class 旋转矩形
	{
		public Point A { get; }

		public Point B { get; }

		public Point C { get; }

		public Point D { get; }

		public 旋转矩形(Point a, Point b, Point c, Point d)
		{
			A = a;
			B = b;
			C = c;
			D = d;
		}

		public bool Contains(Point point)
		{
			Vector vector = point - A;
			Vector vector2 = B - A;
			double num = vector * vector2;
			double num2 = vector2 * vector2;
			Vector vector3 = C - A;
			double num3 = vector * vector3;
			double num4 = vector3 * vector3;
			if (num > 0.0 && num < num2 && num3 > 0.0 && num3 < num4)
			{
				return true;
			}
			return false;
		}
	}
}
