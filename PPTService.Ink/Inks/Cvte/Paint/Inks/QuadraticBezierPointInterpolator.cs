using System;
using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Inks
{
	public class QuadraticBezierPointInterpolator
	{
		private Point? _p1;

		private Point? _p2;

		private Point? _p3;

		public bool IsEnabled { get; set; } = true;

		public int? InterpolationCount { get; set; } = 2;

		public double InterpolationDistanceThreashold { get; set; } = 40.0;

		public Point Start(Point point)
		{
			_p1 = point;
			_p2 = null;
			_p3 = null;
			return point;
		}

		public IEnumerable<Point> Next(Point point)
		{
			if (!IsEnabled)
			{
				yield return point;
				yield break;
			}
			if (!_p1.HasValue)
			{
				_p1 = point;
				yield break;
			}
			if (!_p2.HasValue)
			{
				_p2 = point;
				yield break;
			}
			_p3 = point;
			Point end = new Point((_p2.Value.X + _p3.Value.X) / 2.0, (_p2.Value.Y + _p3.Value.Y) / 2.0);
			IEnumerable<Point> enumerable = CalculateBezierPoints(_p1.Value, _p2.Value, end);
			foreach (Point item in enumerable)
			{
				yield return item;
			}
			_p1 = end;
			_p2 = _p3;
		}

		public void End()
		{
			_p1 = null;
			_p2 = null;
			_p3 = null;
		}

		public IEnumerable<Point> End(Point point)
		{
			foreach (Point item in Next(point))
			{
				yield return item;
			}
			End();
		}

		private IEnumerable<Point> CalculateBezierPoints(Point begin, Point handle, Point end)
		{
			int? interpolationCount = InterpolationCount;
			double count;
			if (interpolationCount.HasValue)
			{
				int valueOrDefault = interpolationCount.GetValueOrDefault();
				count = valueOrDefault;
			}
			else
			{
				double x = begin.X;
				double y = begin.Y;
				double x2 = end.X;
				double y2 = end.Y;
				double x3 = handle.X;
				double y3 = handle.Y;
				double num = Math.Sqrt((x - x3) * (x - x3) + (y - y3) * (y - y3));
				double num2 = Math.Sqrt((x2 - x3) * (x2 - x3) + (y2 - y3) * (y2 - y3));
				double num3 = num + num2;
				count = num3 / InterpolationDistanceThreashold;
				if (count < 2.0)
				{
					count = 2.0;
				}
			}
			double step = 1.0 / count;
			double t = 0.0;
			for (int i = 0; (double)i < count; i++)
			{
				yield return GetBezierPointByT(begin, handle, end, t);
				t += step;
			}
		}

		private Point GetBezierPointByT(Point begin, Point handle, Point end, double t)
		{
			double num = Math.Pow(1.0 - t, 2.0);
			double x = num * begin.X + 2.0 * t * (1.0 - t) * handle.X + t * t * end.X;
			double y = num * begin.Y + 2.0 * t * (1.0 - t) * handle.Y + t * t * end.Y;
			return new Point(x, y);
		}
	}
}
