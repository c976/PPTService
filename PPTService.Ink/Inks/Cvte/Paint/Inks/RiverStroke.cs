using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks
{
	public sealed class RiverStroke : RoadStrokeBase
	{
		private Pen _smallPen;

		private static bool _isDown;

		private const int StrokeThinkness = 4;

		private const int RiverHeight = 10;

		private const int RiverStepLength = 10;

		private readonly Color _color = Color.FromRgb(72, 210, byte.MaxValue);

		public static bool IsDirectionInit;

		public RiverStroke([NotNull] StylusPointCollection stylusPoints, double scale = 1.0)
			: base(stylusPoints, scale)
		{
			Scale = scale;
			InitialDrawingAttributes(_color, 4.0 * Scale);
			InitialDrawBehaviors();
		}

		protected override void InitialDrawingAttributes(Color color, double width)
		{
			base.InitialDrawingAttributes(color, width);
			_smallPen = new Pen(Brush, width / 2.0);
		}

		protected override void InitialDrawBehaviors()
		{
			StepLength = 10.0 * Scale;
			RoadHeight = 10.0 * Scale;
			DrawingAttributes.Width = 24.0 * Scale;
			DrawingAttributes.Height = 24.0 * Scale;
			PrePoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
			DrawActions = new List<Action<DrawingContext, Point>> { DrawAction };
		}

		private void CheckDirection(Point point)
		{
			if (!IsDirectionInit)
			{
				Vector vector = Point.Subtract(point, PrePoint);
				if (!(vector.Length < StepLength / 2.0))
				{
					_isDown = vector.X < 0.0;
					IsDirectionInit = true;
				}
			}
		}

		protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
		{
			PrePoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
			foreach (StylusPoint stylusPoint in StylusPoints)
			{
				Point point = (Point)stylusPoint;
				if (CheckPoint(point))
				{
					CheckDirection(point);
					DrawPoint(drawingContext, point);
				}
			}
		}

		private void DrawAction(DrawingContext drawingContext, Point point)
		{
			if (CheckPoint(point))
			{
				double length = (_isDown ? (0.0 - RoadHeight) : RoadHeight);
				Point point2 = PointExtensions.CalcEndVerticalPoint(PrePoint, point, length);
				drawingContext.DrawLine(_smallPen, point, point2);
				drawingContext.DrawLine(Pen, PrePoint, point);
				PrePoint = point;
			}
		}
	}
}
