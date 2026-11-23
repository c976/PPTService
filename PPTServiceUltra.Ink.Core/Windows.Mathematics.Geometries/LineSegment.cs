using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.Mathematics.Geometries
{
	public struct LineSegment
	{
		public Line Line
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException("此线段不是有意义的线段。");
				}
				return new Line(Point1, Point2);
			}
		}

		public bool IsValid => !Nearly.Equals(Point1, Point2);

		public Point Point1
		{
			
			get;
		}

		public Point Point2
		{
			
			get;
		}

		public Rect Bounds => new Rect(Point1, Point2);

		public LineSegment(Point point1, Point point2)
		{
			Point1 = point1;
			Point2 = point2;
		}

		public bool Contains(Point point)
		{
			double num = 1E-05;
			if (Line.Contains(point) && point.X <= Math.Max(Point1.X, Point2.X) + num && point.X >= Math.Min(Point1.X, Point2.X) - num && point.Y <= Math.Max(Point1.Y, Point2.Y) + num)
			{
				return point.Y >= Math.Min(Point1.Y, Point2.Y) - num;
			}
			return false;
		}

		public static double AngleBetween(LineSegment lineSegment1, LineSegment lineSegment2)
		{
			if (!lineSegment1.IsValid)
			{
				throw new ArgumentException("lineSegment1不是有意义的线段。", "lineSegment1");
			}
			if (!lineSegment2.IsValid)
			{
				throw new ArgumentException("lineSegment2不是有意义的线段。", "lineSegment2");
			}
			return Line.AngleBetween(lineSegment1.Line, lineSegment2.Line);
		}

		public static Point? Intersect(LineSegment lineSegment1, LineSegment lineSegment2)
		{
			if (!lineSegment1.IsValid)
			{
				throw new ArgumentException("lineSegment1不是有意义的线段。", "lineSegment1");
			}
			if (!lineSegment2.IsValid)
			{
				throw new ArgumentException("lineSegment2不是有意义的线段。", "lineSegment2");
			}
			Point? result = Line.Intersect(lineSegment1.Line, lineSegment2.Line);
			if (result.HasValue && lineSegment1.Contains(result.Value) && lineSegment2.Contains(result.Value))
			{
				return result;
			}
			return null;
		}

		public static bool AreParrallel(LineSegment lineSegment1, LineSegment lineSegment2)
		{
			if (!lineSegment1.IsValid)
			{
				throw new ArgumentException("lineSegment1不是有意义的线段。", "lineSegment1");
			}
			if (!lineSegment2.IsValid)
			{
				throw new ArgumentException("lineSegment2不是有意义的线段。", "lineSegment2");
			}
			return Line.AreParallel(lineSegment1.Line, lineSegment2.Line);
		}

		public double[] LineExpression()
		{
			if (Nearly.Equals(Point1.X, Point2.X) && Nearly.Equals(Point1.Y, Point2.Y))
			{
				return new double[3];
			}
			if (Nearly.Equals(Point1.X, Point2.X) && !Nearly.Equals(Point1.Y, Point2.Y))
			{
				return new double[3]
				{
					1.0,
					0.0,
					0.0 - Point1.X
				};
			}
			if (!Nearly.Equals(Point1.X, Point2.X) && Nearly.Equals(Point1.Y, Point2.Y))
			{
				return new double[3]
				{
					0.0,
					1.0,
					0.0 - Point1.Y
				};
			}
			double num = Point2.Y - Point1.Y;
			double num2 = Point1.X - Point2.X;
			double num3 = Point1.X * (Point1.Y - Point2.Y) + Point1.Y * (Point2.X - Point1.X);
			return new double[3] { num, num2, num3 };
		}

		public bool Equals(LineSegment segment)
		{
			if (!(segment.Point1 == Point1) || !(segment.Point2 == Point2))
			{
				if (segment.Point1 == Point2)
				{
					return segment.Point2 == Point1;
				}
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is LineSegment)
			{
				return Equals((LineSegment)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Point1.GetHashCode() ^ Point2.GetHashCode();
		}
	}
}
