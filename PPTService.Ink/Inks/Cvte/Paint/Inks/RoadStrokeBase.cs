using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks
{
	public abstract class RoadStrokeBase : StrokeBase, IRoadStroke
	{
		private int _index;

		protected double RoadHeight { get; set; }

		protected Brush Brush { get; set; }

		protected Pen Pen { get; set; }

		protected Point PrePoint { get; set; }

		public double StepLength { get; protected set; }

		public int StepAmount
		{
			get
			{
				if (DrawActions == null)
				{
					return -1;
				}
				return DrawActions.Count;
			}
		}

		public IList<Action<DrawingContext, Point>> DrawActions { get; set; }

		protected double Scale { get; set; } = 1.0;

		protected RoadStrokeBase([NotNull] StylusPointCollection stylusPoints, double scale)
			: base(stylusPoints)
		{
			if (scale <= 0.0)
			{
				throw new ArgumentException($"{scale}必须为正");
			}
			Scale = scale;
		}

		protected RoadStrokeBase([NotNull] StylusPointCollection stylusPoints, [NotNull] DrawingAttributes drawingAttributes)
			: base(stylusPoints, drawingAttributes)
		{
		}

		protected virtual void InitialDrawingAttributes(Color color, double width)
		{
			Brush = new SolidColorBrush(color);
			Pen = new Pen(Brush, width)
			{
				StartLineCap = PenLineCap.Square,
				EndLineCap = PenLineCap.Square
			};
		}

		protected abstract void InitialDrawBehaviors();

		protected bool CheckPoint(Point point)
		{
			if (point.X.Equals(double.NegativeInfinity) || point.Y.Equals(double.NegativeInfinity))
			{
				return false;
			}
			if (PrePoint.X.Equals(double.NegativeInfinity) || PrePoint.Y.Equals(double.NegativeInfinity))
			{
				PrePoint = point;
				_index = 0;
				return false;
			}
			return true;
		}

		protected void DrawPoint(DrawingContext drawingContext, Point pt)
		{
			if (DrawActions == null)
			{
				throw new InvalidOperationException("必须先设置DrawActions才可以调用");
			}
			DrawActions[_index](drawingContext, pt);
			EnsureIndex();
		}

		private void EnsureIndex()
		{
			_index++;
			if (_index >= StepAmount)
			{
				_index = 0;
			}
		}
	}
}
