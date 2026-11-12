using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Cvte.Paint.Inks.BrushPen_
{
	internal class BrushStrokeOptimizer
	{
		private const double RememberingFactor = 0.95;

		private const int ModifyStylusTipPointCount = 17;

		private const int HistoryCount = 30;

		private const double PowFactor = 0.4;

		private const double MinFactor = 0.3;

		private const int HeadPointCount = 30;

		private const double StrokeHeadMagnifingRatio = 1.8;

		private const int RearPointCount = 20;

		private const double GradientRatio = 0.5;

		private const double AdjoinRadiusRatio = 5.0;

		private const double AdjoinRememberRatio = 0.3;

		private const double DeflectingFactor = 100.0;

		private const float FirstPointPressureFactor = 0.2f;

		private readonly Queue<double> _queue = new Queue<double>();

		private bool _done;

		public Stroke Stroke { get; private set; }

		public BrushStrokeOptimizer(Stroke stroke)
		{
			Stroke = stroke;
			StylusPoint[] source = stroke.StylusPoints.ToArray();
			stroke.StylusPoints = new StylusPointCollection(source.Select((StylusPoint x) => new StylusPoint(x.X, x.Y, 0.2f)));
			Matrix stylusTipTransform = default(Matrix);
			stylusTipTransform.Scale(0.5, 1.0);
			Random random = new Random();
			stylusTipTransform.Rotate(-45.0 + 15.0 * (random.NextDouble() - 1.0));
			Stroke.DrawingAttributes.StylusTip = StylusTip.Ellipse;
			Stroke.DrawingAttributes.StylusTipTransform = stylusTipTransform;
			Stroke.DrawingAttributes.Width *= 0.5;
			Stroke.DrawingAttributes.Width += 3.0;
			Stroke.DrawingAttributes.Height *= 0.5;
			Stroke.DrawingAttributes.Height += 3.0;
		}

		public IEnumerable<StylusPoint> Add(StylusPointCollection stylusPoints)
		{
			return stylusPoints.Select(Add).ToArray();
		}

		public StylusPoint Add(StylusPoint stylusPoint)
		{
			double num = stylusPoint.PressureFactor;
			Point point = stylusPoint.ToPoint();
			Stroke.StylusPoints.Last().ToPoint();
			Point point2 = point + default(Vector);
			_queue.Enqueue(num);
			if (_queue.Count >= 30)
			{
				_queue.Dequeue();
			}
			num = _queue.Average() * 0.95 + num * 0.050000000000000044;
			num = Math.Pow(num, 0.4);
			num = Math.Max(num, 0.3);
			num = 0.3 / num;
			num -= 0.05;
			num *= 1.05;
			num = Math.Min(num, 1.0);
			num = Math.Max(num, 0.05);
			Vector vector = stylusPoint.ToPoint() - Stroke.StylusPoints[0].ToPoint();
			if (Stroke.StylusPoints.Count == 30 || vector.Length > 30.0)
			{
				AdjustHead(30, num);
			}
			if (Stroke.StylusPoints.Count == 17 && vector.X > 0.0 && vector.Y / vector.X > 0.15)
			{
				Matrix stylusTipTransform = default(Matrix);
				stylusTipTransform.Scale(0.5, 1.0);
				stylusTipTransform.Rotate(30.0);
				Stroke.DrawingAttributes.StylusTip = StylusTip.Ellipse;
				Stroke.DrawingAttributes.StylusTipTransform = stylusTipTransform;
			}
			double num2 = num;
			if (GetAdjoinPoints(stylusPoint).Any())
			{
				num2 = GetAdjoinPoints(stylusPoint).Max((StylusPoint x) => x.PressureFactor);
				if (num2 < num)
				{
					num2 = num;
				}
			}
			stylusPoint = new StylusPoint(point2.X, point2.Y, (float)(num * 0.7 + num2 * 0.3));
			Stroke.StylusPoints.Add(stylusPoint);
			return stylusPoint;
		}

		public void Complete()
		{
			AdjustRear();
			if (Stroke.StylusPoints.Count < 30)
			{
				AdjustHead(Stroke.StylusPoints.Count);
			}
		}

		private void AdjustHead(int pointCount, double jointFactor = double.NaN)
		{
			if (!_done)
			{
				_done = true;
				pointCount = Math.Max(pointCount, Stroke.StylusPoints.Count);
				StylusPoint[] array = Stroke.StylusPoints.Take(pointCount).ToArray();
				float average = array.Average((StylusPoint x) => x.PressureFactor);
				double num = -0.5;
				num++;
				double ratio = jointFactor / (double)average / num;
				if (double.IsNaN(jointFactor))
				{
					ratio = 1.8;
				}
				Stroke.StylusPoints.Add(new StylusPointCollection(array.Select((StylusPoint p, int i) =>
				{
					double num3 = (double)i / (double)pointCount * 0.5;
					num3 = 0.0 - num3;
					num3++;
					num3 = num3 * (double)average * ratio;
					num3 = Math.Min(num3, 1.0);
					return new StylusPoint(p.X, p.Y, (float)num3);
				})));
				StylusPoint[] array2 = array;
				foreach (StylusPoint item in array2)
				{
					Stroke.StylusPoints.Remove(item);
				}
			}
		}

		private void AdjustRear()
		{
			if (Stroke.StylusPoints.Count >= 20)
			{
				StylusPoint[] array = Stroke.StylusPoints.Reverse().Take(19).ToArray();
				List<StylusPoint> list = new List<StylusPoint>();
				for (int i = 0; i < array.Length - 1; i++)
				{
					StylusPoint item = array[i];
					StylusPoint stylusPoint = array[i + 1];
					StylusPoint item2 = new StylusPoint((item.X + stylusPoint.X) / 2.0, (item.Y + stylusPoint.Y) / 2.0, (item.PressureFactor + stylusPoint.PressureFactor) / 2f);
					list.Add(item2);
					list.Add(item);
				}
				list.Add(array.Last());
				IEnumerable<StylusPoint> stylusPoints = list.Select((StylusPoint p, int num2) => new StylusPoint(p.X, p.Y, p.PressureFactor * (float)num2 / 19f / 2f)).Reverse();
				Stroke.StylusPoints.Add(new StylusPointCollection(stylusPoints));
				StylusPoint[] array2 = array;
				foreach (StylusPoint item3 in array2)
				{
					Stroke.StylusPoints.Remove(item3);
				}
			}
		}

		private IEnumerable<StylusPoint> GetAdjoinPoints(StylusPoint stylusPoint)
		{
			double radius = 5.0 * Stroke.DrawingAttributes.Width;
			return Stroke.StylusPoints.Reverse().TakeWhile((StylusPoint x) => LengthBetween(x, stylusPoint) < radius);
		}

		private static double LengthBetween(StylusPoint leftPoint, StylusPoint rightPoint)
		{
			return (leftPoint.ToPoint() - rightPoint.ToPoint()).Length;
		}
	}
}
