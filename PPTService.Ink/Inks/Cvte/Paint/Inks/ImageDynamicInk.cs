using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cvte.Diagnostics.Annotations;
using Cvte.Linq;
using Cvte.Paint.Framework;
using Cvte.Paint.Settings;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Inks
{
	public class ImageDynamicInk : DynamicInk
	{
		private readonly StylusPointCollection _drawPoints;

		private readonly BitmapImage _inkImage;

		private readonly double _strokeWidth;

		private readonly double _strokeHeight;

		private readonly double _bitmapScale;

		private StylusPoint _beginPoint;

		public ImageDynamicInk(UIElement adornedElement, InkBoardSettings state, double stateInkScale)
			: base(adornedElement, state)
		{
			IsHitTestVisible = false;
			_inkImage = state.InkImage;
			_bitmapScale = stateInkScale;
			_strokeWidth = (double)state.InkImage.PixelWidth * _bitmapScale;
			_strokeHeight = (double)state.InkImage.PixelHeight * _bitmapScale;
			_drawPoints = new StylusPointCollection();
		}

		protected override void OnAppendPoint(StylusPoint point, bool isLastPoint)
		{
			Points.Add(point);
			StylusPoint? stylusPoint = CalculateDrawPoint(point);
			if (stylusPoint.HasValue)
			{
				Point topLeftPoint = GetTopLeftPoint(stylusPoint.Value.ToPoint());
				StylusPoint stylusPoint2 = new StylusPoint(topLeftPoint.X, topLeftPoint.Y);
				DrawPoint(stylusPoint2);
				_drawPoints.Add(stylusPoint2);
				_beginPoint = stylusPoint2;
				base.OnAppendPoint(point, isLastPoint);
			}
		}

		[Pure]
		public override Element AsStatic()
		{
			Point topLeft = GetTopLeftPoint();
			return new ImageInk(new ImageStroke(new StylusPointCollection(_drawPoints.Select((StylusPoint point) => new StylusPoint(point.X - topLeft.X, point.Y - topLeft.Y, point.PressureFactor))), _inkImage, _bitmapScale)
			{
				StrokePointDetailsList = StrokePointDetailsList
			})
			{
				Location = topLeft
			};
		}

		private Point GetTopLeftPoint()
		{
			Rect strokeBounds = Rect.Empty;
			_drawPoints.ForEach((StylusPoint x) =>
			{
				strokeBounds.Union(x.ToPoint());
			});
			return SnapsToDevicePixel(strokeBounds.Expand(_strokeWidth / 2.0, _strokeHeight / 2.0, _strokeWidth / 2.0, _strokeHeight / 2.0).TopLeft);
		}

		private void DrawPoint(StylusPoint point)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				drawingContext.DrawImage(rectangle: new Rect(point.X - _strokeWidth / 2.0, point.Y - _strokeHeight / 2.0, _strokeWidth, _strokeHeight), imageSource: _inkImage);
			}
			AddVisual(drawingVisual);
		}

		private StylusPoint? CalculateDrawPoint(StylusPoint point)
		{
			if (Points.Count < 2)
			{
				return point;
			}
			Rect selfCentreRect = GetSelfCentreRect(_beginPoint, _strokeWidth, _strokeHeight);
			Point? point2 = LineSegmentInterscetsWithRect(new Cvte.Windows.Mathematics.Geometries.LineSegment(_beginPoint.ToPoint(), point.ToPoint()), selfCentreRect);
			if (!point2.HasValue)
			{
				return null;
			}
			return VectorPointTo(_beginPoint.ToPoint(), point2.Value, Math.Max(_strokeWidth, _strokeHeight));
		}

		private Point GetTopLeftPoint(Point point)
		{
			Vector vector = new Vector(_strokeWidth / 2.0, _strokeWidth / 2.0);
			Point point2 = point - vector;
			return SnapsToDevicePixel(point2) + vector;
		}

		private Point SnapsToDevicePixel(Point point)
		{
			Point point2 = AdornedElement.PointToScreen(point);
			return AdornedElement.PointFromScreen(point2);
		}

		private static StylusPoint VectorPointTo(Point a, Point b, double len)
		{
			Vector vector = Point.Subtract(a, b);
			Point point;
			if (vector.Length * 2.0 > len)
			{
				vector.Normalize();
				vector = Vector.Multiply(vector, len);
				point = Point.Subtract(a, vector);
			}
			else
			{
				point = Point.Subtract(b, vector);
			}
			return new StylusPoint(point.X, point.Y);
		}

		private static Rect GetSelfCentreRect(StylusPoint point, double width, double height)
		{
			return new Rect(point.X - width / 2.0, point.Y - height / 2.0, width, height);
		}

		private static Point? LineSegmentInterscetsWithRect(Cvte.Windows.Mathematics.Geometries.LineSegment s, Rect rect)
		{
			if (!s.IsValid)
			{
				return null;
			}
			foreach (Cvte.Windows.Mathematics.Geometries.LineSegment item in new List<Cvte.Windows.Mathematics.Geometries.LineSegment>
			{
				new Cvte.Windows.Mathematics.Geometries.LineSegment(rect.TopRight, rect.BottomRight),
				new Cvte.Windows.Mathematics.Geometries.LineSegment(rect.BottomRight, rect.BottomLeft),
				new Cvte.Windows.Mathematics.Geometries.LineSegment(rect.TopLeft, rect.TopRight),
				new Cvte.Windows.Mathematics.Geometries.LineSegment(rect.BottomLeft, rect.TopLeft)
			})
			{
				Point? result = Cvte.Windows.Mathematics.Geometries.LineSegment.Intersect(item, s);
				if (result.HasValue)
				{
					return result;
				}
			}
			return null;
		}
	}
}
