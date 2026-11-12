using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Windows;

namespace Cvte.Paint.Framework
{
	public static class SnapDrawingExtensions
	{
		public static void DrawSnappedRectangle([NotNull] this DrawingContext dc, [CanBeNull] Brush fill, [CanBeNull] Pen pen, Rect rect)
		{
			if (dc == null)
			{
				throw new ArgumentNullException("dc");
			}
			dc.PushGuidelineSet(new GuidelineSet(new double[2] { rect.Left, rect.Right }, new double[2] { rect.Top, rect.Bottom }));
			if (fill != null)
			{
				Rect rectangle = rect;
				dc.DrawRectangle(fill, null, rectangle);
			}
			if (pen != null)
			{
				double thickness = pen.Thickness;
				double num = thickness / 2.0;
				double x = rect.Left + num;
				double y = rect.Top + num;
				double num2 = rect.Width - thickness;
				double num3 = rect.Height - thickness;
				num2 = ((num2 < 0.0) ? 0.0 : num2);
				num3 = ((num3 < 0.0) ? 0.0 : num3);
				Rect rectangle2 = new Rect(x, y, num2, num3);
				dc.DrawRectangle(null, pen, rectangle2);
			}
			dc.Pop();
		}

		public static void DrawSnappedLine([NotNull] this DrawingContext dc, [CanBeNull] Pen pen, Point startPoint, Point endPoint)
		{
			if (dc == null)
			{
				throw new ArgumentNullException("dc");
			}
			if (pen != null)
			{
				double thickness = pen.Thickness;
				double num = thickness / 2.0;
				dc.PushGuidelineSet(new GuidelineSet(new double[2] { startPoint.X, endPoint.X }, new double[2] { startPoint.Y, endPoint.Y }));
				double num2;
				double num3;
				if (startPoint.X - thickness > endPoint.X)
				{
					num2 = num;
					num3 = 0.0 - num;
				}
				else if (startPoint.X + thickness < endPoint.X)
				{
					num2 = 0.0 - num;
					num3 = num;
				}
				else
				{
					num2 = num;
					num3 = num;
				}
				double num4;
				double num5;
				if (startPoint.Y - thickness > endPoint.Y)
				{
					num4 = num;
					num5 = 0.0 - num;
				}
				else if (startPoint.Y + thickness < endPoint.Y)
				{
					num4 = 0.0 - num;
					num5 = num;
				}
				else
				{
					num4 = num;
					num5 = num;
				}
				dc.DrawLine(pen, new Point(startPoint.X + num2, startPoint.Y + num4), new Point(endPoint.X + num3, endPoint.Y + num5));
				dc.Pop();
			}
		}

		public static void DrawSnappedLinesBetweenPoints([NotNull] this DrawingContext dc, Pen pen, params Point[] points)
		{
			if (dc == null)
			{
				throw new ArgumentNullException("dc");
			}
			GuidelineSet guidelineSet = new GuidelineSet();
			Point[] array = points;
			for (int i = 0; i < array.Length; i++)
			{
				Point point = array[i];
				guidelineSet.GuidelinesX.Add(point.X);
				guidelineSet.GuidelinesY.Add(point.Y);
			}
			double half = pen.Thickness / 2.0;
			points = points.Select((Point p) => new Point(p.X + half, p.Y + half)).ToArray();
			dc.PushGuidelineSet(guidelineSet);
			for (int num = 0; num < points.Length - 1; num += 2)
			{
				dc.DrawLine(pen, points[num], points[num + 1]);
			}
			dc.Pop();
		}

		public static double GetRoundDpiScaledThickness(double thickness)
		{
			double factorX = Dpi.System.FactorX;
			thickness /= factorX;
			factorX = (int)Math.Round(factorX);
			factorX = ((factorX <= 0.0) ? 1.0 : factorX);
			thickness *= factorX;
			return thickness;
		}
	}
}
