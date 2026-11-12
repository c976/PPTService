using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Inks
{
	public static class BezierInterpolation
	{
		private static StylusPointCollection InterpolateInner(StylusPoint p0, StylusPoint p1, StylusPointCollection newPoints, double bezierInterpolationThreshold = 5.0)
		{
			StylusPointCollection stylusPointCollection = new StylusPointCollection(newPoints.Description);
			foreach (StylusPoint newPoint in newPoints)
			{
				int num = (int)((V(newPoint) - V(p1)).Length / bezierInterpolationThreshold);
				if (num == 0)
				{
					stylusPointCollection.Add(newPoint);
					p0 = p1;
					p1 = newPoint;
					continue;
				}
				List<StylusPoint> quadraticBezierPoints = GetQuadraticBezierPoints(p0, p1, newPoint, num);
				foreach (StylusPoint item in quadraticBezierPoints)
				{
					stylusPointCollection.Add(item);
				}
				p0 = quadraticBezierPoints[quadraticBezierPoints.Count - 2];
				p1 = newPoint;
			}
			if (!stylusPointCollection.Any())
			{
				stylusPointCollection.Add(newPoints.Last());
			}
			return stylusPointCollection;
		}

		private static StylusPoint GetQuadraticBezierPoint(StylusPoint p0, StylusPoint p1, StylusPoint p2, double t)
		{
			StylusPoint result = p0;
			Vector vector = V(p0);
			Vector vector2 = V(p1);
			Vector vector3 = V(p2);
			Vector vector4 = (1.0 - t) * (1.0 - t) * vector + 2.0 * (1.0 - t) * t * vector2 + t * t * vector3;
			result.X = vector4.X;
			result.Y = vector4.Y;
			result.PressureFactor = p0.PressureFactor + (float)t * (p2.PressureFactor - p0.PressureFactor);
			return result;
		}

		private static StylusPoint GetQuadraticBezierP1(StylusPoint p0, StylusPoint pt, StylusPoint p2, double t)
		{
			StylusPoint result = p0;
			Vector vector = V(p0);
			Vector vector2 = V(pt);
			Vector vector3 = V(p2);
			Vector vector4 = (vector2 - t * t * vector3 - (1.0 - t) * (1.0 - t) * vector) / (2.0 * (1.0 - t) * t);
			StylusPoint stylusPoint = p0;
			result.X = vector4.X;
			result.Y = vector4.Y;
			result.PressureFactor = p0.PressureFactor + (float)t * (stylusPoint.PressureFactor - p0.PressureFactor);
			return result;
		}

		private static Vector V(StylusPoint p)
		{
			return new Vector(p.X, p.Y);
		}

		private static List<StylusPoint> GetQuadraticBezierPoints(StylusPoint p0, StylusPoint pt, StylusPoint p2, int steps)
		{
			List<StylusPoint> list = new List<StylusPoint>();
			double quadraticBezierT = GetQuadraticBezierT(p0, pt, p2);
			StylusPoint quadraticBezierP = GetQuadraticBezierP1(p0, pt, p2, quadraticBezierT);
			for (int i = 1; i <= steps; i++)
			{
				double t = quadraticBezierT + (double)i / ((double)steps + 1.0) * (1.0 - quadraticBezierT);
				StylusPoint quadraticBezierPoint = GetQuadraticBezierPoint(p0, quadraticBezierP, p2, t);
				list.Add(quadraticBezierPoint);
			}
			list.Add(p2);
			return list;
		}

		private static double GetQuadraticBezierT(StylusPoint p0, StylusPoint pt, StylusPoint p2)
		{
			double num = (V(pt) - V(p0)).Length / ((V(pt) - V(p0)).Length + (V(p2) - V(pt)).Length);
			if (Math.Abs(Vector.AngleBetween(V(pt) - V(p0), V(p2) - V(pt))) < 4.0)
			{
				return num + (1.0 - num) * 0.2;
			}
			return num + (1.0 - num) * 0.05;
		}

		private static IEnumerable<StylusPoint> FilterClosedPoints(StylusPointCollection points, StylusPoint firstPoint, double tolerance)
		{
			StylusPoint current = firstPoint;
			foreach (StylusPoint point in points)
			{
				if (!Nearly.Equals(point.ToPoint(), current.ToPoint(), tolerance))
				{
					current = point;
					yield return point;
				}
			}
		}

		public static StylusPointCollection Interpolate(StylusPointCollection newPoints, IList<StylusPoint> collectedStylusPoints, double bezierInterpolationThreshold = 5.0, double pointMinDistance = 0.5)
		{
			if (collectedStylusPoints.Count == 0)
			{
				return newPoints;
			}
			StylusPoint stylusPoint = collectedStylusPoints[collectedStylusPoints.Count - 1];
			StylusPoint[] array = FilterClosedPoints(newPoints, stylusPoint, pointMinDistance).ToArray();
			StylusPointCollection stylusPointCollection = (array.Any() ? new StylusPointCollection(array) : new StylusPointCollection(newPoints.Description));
			if (!stylusPointCollection.Any())
			{
				return stylusPointCollection;
			}
			if (collectedStylusPoints.Count == 1)
			{
				return stylusPointCollection;
			}
			return InterpolateInner(collectedStylusPoints[collectedStylusPoints.Count - 2], stylusPoint, stylusPointCollection, bezierInterpolationThreshold);
		}
	}
}
