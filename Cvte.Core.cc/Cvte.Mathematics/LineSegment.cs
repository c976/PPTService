using System;
using System.Windows;

namespace Cvte.Mathematics
{
	public struct LineSegment
	{
		private readonly Point _point1;

		private readonly Point _point2;

		public Line Line
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException("此线段不是有意义的线段。");
				}
				return new Line(_point1, _point2);
			}
		}

		public bool IsValid => !Nearly.Equals(Point1, Point2);

		public Point Point1 => _point1;

		public Point Point2 => _point2;

		public Rect Bounds => new Rect(Point1, Point2);

		public LineSegment(Point point1, Point point2)
		{
			_point1 = point1;
			_point2 = point2;
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

		public bool AreParrallel(LineSegment lineSegment1, LineSegment lineSegment2)
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

		public bool Equals(LineSegment segment)
		{
			if (segment._point1 == _point1 && segment._point2 == _point2)
			{
				return true;
			}
			if (segment._point1 == _point2)
			{
				return segment._point2 == _point1;
			}
			return false;
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
			return _point1.GetHashCode() ^ _point2.GetHashCode();
		}
	}
}
