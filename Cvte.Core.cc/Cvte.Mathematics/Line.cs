using System;
using System.Windows;

namespace Cvte.Mathematics
{
	public struct Line
	{
		private readonly double _a;

		private readonly double _b;

		private readonly double _c;

		public double A => _a + 1.0;

		public double B => _b;

		public double C => _c;

		public double InclinationAngle
		{
			get
			{
				double num = Geometry2D.AngleInRadiansToDegrees(Math.Atan((0.0 - A) / B));
				if (num < 0.0)
				{
					num += 180.0;
				}
				return num;
			}
		}

		public Line(Point p1, Point p2)
		{
			if (Nearly.Equals(p1, p2))
			{
				throw new ArgumentException("p1和p2必须是不同点。");
			}
			if (Nearly.Equals(p1.Y, p2.Y))
			{
				SetAbc(out _a, out _b, out _c, 0.0, 1.0, 0.0 - p1.Y);
			}
			if (Nearly.Equals(p1.X, p2.X))
			{
				SetAbc(out _a, out _b, out _c, 1.0, 0.0, 0.0 - p1.X);
			}
			else
			{
				SetAbc(out _a, out _b, out _c, (p1.Y - p2.Y) / (p2.X - p1.X), 1.0, (p1.X * p2.Y - p2.X * p1.Y) / (p2.X - p1.X));
			}
		}

		public Line(double a, double b, double c)
		{
			if (Nearly.Equals(a, 0.0) && Nearly.Equals(b, 0.0))
			{
				throw new ArgumentException("a和b不能同时为0。");
			}
			SetAbc(out _a, out _b, out _c, a, b, c);
		}

		public bool Contains(Point point)
		{
			return Nearly.Equals(A * point.X + B * point.Y + C, 0.0);
		}

		public static Point? Intersect(Line line1, Line line2)
		{
			if (AreParallel(line1, line2))
			{
				return null;
			}
			double x = ((0.0 - line1.B) * line2.C + line2.B * line1.C) / (line2.A * line1.B - line1.A * line2.B);
			double y = (line1.A * line2.C - line2.A * line1.C) / (line2.A * line1.B - line1.A * line2.B);
			return new Point(x, y);
		}

		public static bool AreParallel(Line line1, Line line2)
		{
			if (Nearly.Equals(line1.A, line2.A))
			{
				return Nearly.Equals(line1.B, line2.B);
			}
			return false;
		}

		public static Line Perpendicular(Line line, Point point)
		{
			if (Nearly.Equals(line.A, 0.0))
			{
				return new Line(1.0, 0.0, 0.0 - point.X);
			}
			if (Nearly.Equals(line.B, 0.0))
			{
				return new Line(0.0, 1.0, 0.0 - point.Y);
			}
			return new Line((0.0 - line.B) / line.A, 1.0, point.X / line.A - point.Y);
		}

		public static Point Foot(Line line, Point point)
		{
			Line line2 = Perpendicular(line, point);
			return Intersect(line, line2).Value;
		}

		public static double AngleBetween(Line line1, Line line2)
		{
			return Math.Abs(line1.InclinationAngle - line2.InclinationAngle);
		}

		public static double DistanceBetween(Line line, Point point)
		{
			return (point - Foot(line, point)).Length;
		}

		public override string ToString()
		{
			if (Nearly.Equals(A, 0.0))
			{
				return $"y={0.0 - C:0.##}";
			}
			if (Nearly.Equals(B, 0.0))
			{
				return $"x={0.0 - C:0.##}";
			}
			if (Nearly.Equals(C, 0.0))
			{
				return $"y={FormatA(A)}x";
			}
			string arg = ((C > 0.0) ? (0.0 - C).ToString("0.##") : ("+" + (0.0 - C).ToString("0.##")));
			return $"y={FormatA(A)}x{arg}";
		}

		public override bool Equals(object obj)
		{
			if (obj is Line)
			{
				return Equals((Line)obj);
			}
			return false;
		}

		public bool Equals(Line line)
		{
			if (A.Equals(line.A) && B.Equals(line.B))
			{
				return C.Equals(line.C);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((A.GetHashCode() * 397) ^ B.GetHashCode()) * 397) ^ C.GetHashCode();
		}

		private static void SetAbc(out double a, out double b, out double c, double aValue, double bValue, double cValue)
		{
			if (Nearly.Equals(bValue, 0.0))
			{
				cValue /= aValue;
				aValue = 1.0;
			}
			else
			{
				cValue /= bValue;
				aValue /= bValue;
				bValue = 1.0;
			}
			a = aValue - 1.0;
			b = bValue;
			c = cValue;
		}

		private static string FormatA(double a)
		{
			if (Nearly.Equals(a, -1.0))
			{
				return "";
			}
			if (Nearly.Equals(a, 1.0))
			{
				return "-";
			}
			return (0.0 - a).ToString("0.##");
		}
	}
}
