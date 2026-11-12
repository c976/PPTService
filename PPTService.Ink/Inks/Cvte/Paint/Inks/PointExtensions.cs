using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks
{
	public static class PointExtensions
	{
		public static Point CalcCenterPoint(Point point1, Point point2)
		{
			return new Point((point1.X + point2.X) / 2.0, (point1.Y + point2.Y) / 2.0);
		}

		public static Point CalcCenterVerticalPoint(Point point1, Point point2, double length)
		{
			Vector vector = point2 - point1;
			Point point3 = CalcCenterPoint(point1, point2);
			Vector vector2 = new Vector(vector.Y, 0.0 - vector.X);
			vector2.Normalize();
			return point3 + vector2 * length;
		}

		public static Point CalcCenterVerticalIntegerPoint(Point point1, Point point2, double length)
		{
			return IntegerPoint(CalcCenterVerticalPoint(point1, point2, length));
		}

		public static Point CalcEndVerticalPoint(Point point1, Point point2, double length)
		{
			Vector vector = point2 - point1;
			Vector vector2 = new Vector(vector.Y, 0.0 - vector.X);
			vector2.Normalize();
			return point2 + vector2 * length;
		}

		public static Point CalcEndVerticalIntegerPoint(Point point1, Point point2, double length)
		{
			return IntegerPoint(CalcEndVerticalPoint(point1, point2, length));
		}

		public static Point[] CalcEndSidePoints(Point point1, Point point2, double width)
		{
			Vector vector = point2 - point1;
			Vector vector2 = new Vector(vector.Y, 0.0 - vector.X);
			vector2.Normalize();
			return new Point[2]
			{
				point2 + vector2 * width,
				point2 - vector2 * width
			};
		}

		public static Point[] CalcEndSideIntegerPoints(Point point1, Point point2, double width)
		{
			Point[] array = CalcEndSidePoints(point1, point2, width);
			return new Point[2]
			{
				IntegerPoint(array[0]),
				IntegerPoint(array[1])
			};
		}

		public static Point[] CalcBothUpPoints(Point point1, Point point2, double height)
		{
			Vector vector = point2 - point1;
			Vector vector2 = new Vector(vector.Y, 0.0 - vector.X);
			vector2.Normalize();
			return new Point[2]
			{
				point1 + vector2 * height,
				point2 + vector2 * height
			};
		}

		public static Point[] CalcBothUpIntegerPoints(Point point1, Point point2, double height)
		{
			Point[] array = CalcBothUpPoints(point1, point2, height);
			return new Point[2]
			{
				IntegerPoint(array[0]),
				IntegerPoint(array[1])
			};
		}

		public static PathGeometry GeneratePathGeometry(IList<Point> points)
		{
			PathFigure pathFigure = new PathFigure
			{
				StartPoint = points[0],
				IsClosed = true
			};
			foreach (Point item in points.Skip(1))
			{
				pathFigure.Segments.Add(new LineSegment
				{
					Point = item
				});
			}
			return new PathGeometry(new PathFigure[1] { pathFigure });
		}

		public static Point IntegerPoint(Point point)
		{
			return new Point((int)point.X, (int)point.Y);
		}
	}
}
