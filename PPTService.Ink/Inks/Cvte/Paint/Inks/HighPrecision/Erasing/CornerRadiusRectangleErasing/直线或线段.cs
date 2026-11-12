using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	public struct 直线或线段
	{
		private readonly double _a;

		private readonly double _b;

		private readonly double _c;

		public Point Point1 { get; }

		public Point Point2 { get; }

		public 直线或线段(Point p1, Point p2)
		{
			if (p1 == p2)
			{
				throw new ArgumentException("p2", "线段的起点和终点不能是同一个点。");
			}
			Point1 = p1;
			Point2 = p2;
			_a = p1.Y - p2.Y;
			_b = p2.X - p1.X;
			_c = p1.X * p2.Y - p2.X * p1.Y;
		}

		public double GetDistance(Point point)
		{
			double x = point.X;
			double y = point.Y;
			return Math.Abs(_a * x + _b * y + _c) / Math.Sqrt(_a * _a + _b * _b);
		}

		public 直线或线段 求远离后的新直线(Point reference, double offset)
		{
			double num = Math.Sqrt(_a * _a + _b * _b) / offset;
			double num2 = _a / num;
			double num3 = _b / num;
			直线或线段 result = new 直线或线段(new Point(Point1.X - num2, Point1.Y - num3), new Point(Point2.X - num2, Point2.Y - num3));
			直线或线段 result2 = new 直线或线段(new Point(Point1.X + num2, Point1.Y + num3), new Point(Point2.X + num2, Point2.Y + num3));
			if (result.GetDistance(reference) > result2.GetDistance(reference))
			{
				return result;
			}
			return result2;
		}

		public 直线或线段 求远离后的新直线(直线或线段 reference, double offset)
		{
			double num = Math.Sqrt(_a * _a + _b * _b) / offset;
			double num2 = _a / num;
			double num3 = _b / num;
			直线或线段 result = new 直线或线段(new Point(Point1.X - num2, Point1.Y - num3), new Point(Point2.X - num2, Point2.Y - num3));
			直线或线段 result2 = new 直线或线段(new Point(Point1.X + num2, Point1.Y + num3), new Point(Point2.X + num2, Point2.Y + num3));
			if (result.GetDistance(reference.Point1) > result2.GetDistance(reference.Point1))
			{
				return result;
			}
			return result2;
		}

		public double AngleBetween(直线或线段 other)
		{
			double a = _a;
			double b = _b;
			double a2 = other._a;
			double b2 = other._b;
			double val = Math.Abs((a * a2 + b * b2) / (Math.Sqrt(a * a + b * b) * Math.Sqrt(a2 * a2 + b2 * b2)));
			val = Math.Max(0.0, val);
			val = Math.Min(1.0, val);
			return Math.Acos(val) * 180.0 / Math.PI;
		}
	}
}
