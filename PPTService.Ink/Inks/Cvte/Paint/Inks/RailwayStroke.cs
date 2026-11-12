using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks
{
	public sealed class RailwayStroke : RoadStrokeBase
	{
		private Point _prePointUp;

		private Point _prePointDown;

		private bool _isPreInitial;

		private const int StrokeThinkness = 2;

		private const int RailwayHeight = 10;

		private const int RailwayStepLength = 10;

		private readonly Color _backColor = Color.FromRgb(102, 102, 102);

		private readonly Color _frontColor = Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);

		private readonly Brush _frontBrush;

		private readonly Pen _frontPen;

		public RailwayStroke([NotNull] StylusPointCollection stylusPoints, double scale = 1.0)
			: base(stylusPoints, scale)
		{
			_frontBrush = new SolidColorBrush(_frontColor);
			_frontPen = new Pen(_frontBrush, 1.0 * scale);
			InitialDrawingAttributes(_backColor, 2.0 * scale);
			InitialDrawBehaviors();
		}

		protected override void InitialDrawBehaviors()
		{
			StepLength = 10.0 * Scale;
			RoadHeight = 10.0 * Scale;
			DrawingAttributes.Width = 24.0 * Scale;
			DrawingAttributes.Height = 24.0 * Scale;
			PrePoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
			DrawActions = new List<Action<DrawingContext, Point>> { DrawActionFront, DrawActionFront, DrawActionBack, DrawActionBack };
		}

		private void EnsurePreSidePoints(Point pt)
		{
			if (!_isPreInitial)
			{
				Point[] array = PointExtensions.CalcEndSidePoints(pt, PrePoint, RoadHeight / 2.0);
				_prePointUp = array[1];
				_prePointDown = array[0];
				_isPreInitial = true;
			}
		}

		protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
		{
			PrePoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
			_isPreInitial = false;
			foreach (StylusPoint stylusPoint in StylusPoints)
			{
				Point point = (Point)stylusPoint;
				if (CheckPoint(point))
				{
					EnsurePreSidePoints(point);
					DrawPoint(drawingContext, point);
				}
			}
		}

		private void DrawActionFront(DrawingContext drawingContext, Point point)
		{
			if (CheckPoint(point))
			{
				Point[] array = PointExtensions.CalcEndSidePoints(PrePoint, point, RoadHeight / 2.0);
				PathGeometry geometry = PointExtensions.GeneratePathGeometry(new Point[4]
				{
					_prePointUp,
					array[0],
					array[1],
					_prePointDown
				});
				drawingContext.DrawGeometry(_frontBrush, _frontPen, geometry);
				drawingContext.DrawLine(Pen, _prePointUp, array[0]);
				drawingContext.DrawLine(Pen, _prePointDown, array[1]);
				_prePointUp = array[0];
				_prePointDown = array[1];
				PrePoint = point;
			}
		}

		private void DrawActionBack(DrawingContext drawingContext, Point point)
		{
			if (CheckPoint(point))
			{
				Point[] array = PointExtensions.CalcEndSidePoints(PrePoint, point, RoadHeight / 2.0);
				PathGeometry geometry = PointExtensions.GeneratePathGeometry(new Point[4]
				{
					_prePointUp,
					array[0],
					array[1],
					_prePointDown
				});
				drawingContext.DrawGeometry(Brush, Pen, geometry);
				PrePoint = point;
				_prePointUp = array[0];
				_prePointDown = array[1];
			}
		}
	}
}
