using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks
{
	public sealed class GreatwallStroke : RoadStrokeBase
	{
		private const int StrokeThinkness = 2;

		private const int WallHeight = 10;

		private const int WallStepLength = 10;

		private readonly Color _color = Color.FromRgb(102, 102, 102);

		public GreatwallStroke([NotNull] StylusPointCollection stylusPoints, double scale = 1.0)
			: base(stylusPoints, scale)
		{
			InitialDrawingAttributes(_color, 2.0 * scale);
			InitialDrawBehaviors();
		}

		protected override void InitialDrawBehaviors()
		{
			StepLength = 10.0 * Scale;
			RoadHeight = 10.0 * Scale;
			DrawingAttributes.Width = 24.0 * Scale;
			DrawingAttributes.Height = 24.0 * Scale;
			PrePoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
			DrawActions = new List<Action<DrawingContext, Point>> { DrawActionLine, DrawActionWall };
		}

		protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
		{
			PrePoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
			foreach (StylusPoint stylusPoint in StylusPoints)
			{
				Point point = (Point)stylusPoint;
				if (CheckPoint(point))
				{
					DrawPoint(drawingContext, point);
				}
			}
		}

		private void DrawActionLine(DrawingContext drawingContext, Point point)
		{
			if (CheckPoint(point))
			{
				drawingContext.DrawLine(Pen, PrePoint, point);
				PrePoint = point;
			}
		}

		private void DrawActionWall(DrawingContext drawingContext, Point point)
		{
			if (CheckPoint(point))
			{
				Point[] array = PointExtensions.CalcBothUpPoints(PrePoint, point, RoadHeight);
				drawingContext.DrawLine(Pen, PrePoint, array[0]);
				drawingContext.DrawLine(Pen, array[0], array[1]);
				drawingContext.DrawLine(Pen, array[1], point);
				PrePoint = point;
			}
		}
	}
}
