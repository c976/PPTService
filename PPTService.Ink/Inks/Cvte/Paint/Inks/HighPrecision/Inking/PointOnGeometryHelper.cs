using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	internal static class PointOnGeometryHelper
	{
		public static List<Point> GetPointsOnPath(double abscissa, Geometry geometry)
		{
			List<Point> list = new List<Point>();
			foreach (PathFigure figure in geometry.GetFlattenedPathGeometry().Figures)
			{
				List<Point> ordinateOnPathFigureByAbscissa = GetOrdinateOnPathFigureByAbscissa(figure, abscissa);
				list.AddRange(ordinateOnPathFigureByAbscissa);
			}
			return list;
		}

		private static List<Point> GetOrdinateOnPathFigureByAbscissa(PathFigure figure, double abscissa)
		{
			List<Point> list = new List<Point>();
			Point start = figure.StartPoint;
			foreach (PathSegment segment in figure.Segments)
			{
				PolyLineSegment polyLineSegment = segment as PolyLineSegment;
				LineSegment lineSegment = segment as LineSegment;
				Point[] array;
				if (polyLineSegment != null)
				{
					array = polyLineSegment.Points.ToArray();
				}
				else
				{
					if (lineSegment == null)
					{
						throw new InvalidOperationException();
					}
					array = new Point[1] { lineSegment.Point };
				}
				Point[] array2 = array;
				foreach (Point point in array2)
				{
					double ordinate;
					if (TryGetOrdinateOnVectorByAbscissa(start, point, abscissa, out ordinate))
					{
						list.Add(new Point(abscissa, ordinate));
					}
					start = point;
				}
			}
			return list;
		}

		private static bool TryGetOrdinateOnVectorByAbscissa(Point start, Point end, double abscissa, out double ordinate)
		{
			ordinate = 0.0;
			if ((start.X < end.X && abscissa >= start.X && abscissa <= end.X) || (start.X > end.X && abscissa <= start.X && abscissa >= end.X))
			{
				double num = (abscissa - start.X) / (end.X - start.X);
				double num2 = (end.Y - start.Y) * num + start.Y;
				ordinate = num2;
				return true;
			}
			return false;
		}

		public static Point GetClosestPointOnPath(Point p, Geometry geometry)
		{
			return (from f in geometry.GetFlattenedPathGeometry().Figures
				select GetClosestPointOnPathFigure(f, p) into t
				orderby t.Item2
				select t).FirstOrDefault()?.Item1 ?? new Point(0.0, 0.0);
		}

		private static Tuple<Point, double> GetClosestPointOnPathFigure(PathFigure figure, Point p)
		{
			List<Tuple<Point, double>> list = new List<Tuple<Point, double>>();
			Point start = figure.StartPoint;
			foreach (PathSegment segment in figure.Segments)
			{
				PolyLineSegment polyLineSegment = segment as PolyLineSegment;
				LineSegment lineSegment = segment as LineSegment;
				Point[] array;
				if (polyLineSegment != null)
				{
					array = polyLineSegment.Points.ToArray();
				}
				else
				{
					if (lineSegment == null)
					{
						throw new InvalidOperationException();
					}
					array = new Point[1] { lineSegment.Point };
				}
				Point[] array2 = array;
				foreach (Point point in array2)
				{
					Point closestPointOnLine = GetClosestPointOnLine(start, point, p);
					double lengthSquared = (closestPointOnLine - p).LengthSquared;
					list.Add(new Tuple<Point, double>(closestPointOnLine, lengthSquared));
					start = point;
				}
			}
			return list.OrderBy((Tuple<Point, double> t) => t.Item2).First();
		}

		private static Point GetClosestPointOnLine(Point start, Point end, Point p)
		{
			double lengthSquared = (start - end).LengthSquared;
			if (Math.Abs(lengthSquared) < 0.01)
			{
				return start;
			}
			Vector vector = end - start;
			double num = (p - start) * vector / lengthSquared;
			if (!(num < 0.0))
			{
				if (!(num > 1.0))
				{
					return start + num * vector;
				}
				return end;
			}
			return start;
		}
	}
}
