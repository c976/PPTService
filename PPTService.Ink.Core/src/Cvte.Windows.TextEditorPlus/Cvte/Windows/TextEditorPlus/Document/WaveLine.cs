using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	internal class WaveLine
	{
		public double BorderThickness { get; set; } = 6.0;

		public double WaveLength { get; set; } = 3.0;

		public double WaveHeight { get; set; } = 40.0;

		public double CurveSquaring { get; set; } = 0.75;

		public StreamGeometry DrawWaveLine(Point startPoint, Point endPoint)
		{
			double length = (startPoint - endPoint).Length;
			double num = CalculateAngle(startPoint, endPoint);
			double waveLength = WaveLength;
			double waveHeight = WaveHeight;
			double num2 = length / waveLength;
			double num3 = waveLength;
			double num4 = Math.Sqrt(num3 / 4.0 * (num3 / 4.0) + waveHeight / 2.0 * (waveHeight / 2.0));
			double curveSquaring = CurveSquaring;
			double num5 = num4 * curveSquaring;
			double num6 = CalculateAngle(new Point(0.0, 0.0), new Point(num3 / 4.0, waveHeight / 2.0));
			List<ValueTuple<Point, Point, Point>> list = new List<ValueTuple<Point, Point, Point>>();
			Point point = startPoint;
			int num7 = 1;
			for (int i = 0; (double)i < num2 * 2.0; i++)
			{
				double num8 = num + num6 * (double)num7;
				Point item = new Point(point.X + Math.Cos(num8) * num5, point.Y + Math.Sin(num8) * num5);
				Point point2 = new Point(point.X + Math.Cos(num) * num3 / 2.0, point.Y + Math.Sin(num) * num3 / 2.0);
				double num9 = num + (Math.PI - num6) * (double)num7;
				Point item2 = new Point(point2.X + Math.Cos(num9) * num5, point2.Y + Math.Sin(num9) * num5);
				list.Add(new ValueTuple<Point, Point, Point>(item, item2, point2));
				num7 *= -1;
				point = point2;
			}
			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
			{
				streamGeometryContext.BeginFigure(list[0].Item3, true, false);
				for (int j = 1; j < list.Count; j++)
				{
					ValueTuple<Point, Point, Point> valueTuple = list[j];
					Point item3 = valueTuple.Item1;
					Point item4 = valueTuple.Item2;
					Point item5 = valueTuple.Item3;
					streamGeometryContext.BezierTo(item3, item4, item5, true, false);
				}
				return streamGeometry;
			}
		}

		private static double CalculateAngle(Point p1, Point p2)
		{
			return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
		}
	}
}
