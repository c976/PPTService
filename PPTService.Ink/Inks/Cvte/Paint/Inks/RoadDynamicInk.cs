using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Cvte.Linq;
using Cvte.Paint.Framework;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class RoadDynamicInk : DynamicInk
	{
		private const int StepLength = 10;

		private const int RoadHeight = 10;

		private Point _prePoint;

		private readonly StylusPointCollection _drawPoints;

		private readonly MagicStrokeInfo _strokeInfo;

		public InkType StrokeType => _strokeInfo.StrokeType;

		public int SegmentPointCount { get; set; } = 20;

		protected RoadStrokeVisual CurrentStrokeVisual { get; set; }

		public RoadDynamicInk(UIElement adornedElement, InkBoardSettings state, MagicStrokeInfo info)
			: base(adornedElement, DynamicInk.CreateDrawingAttributes(state))
		{
			_strokeInfo = info;
			_drawPoints = new StylusPointCollection();
			CreateNewVisual();
		}

		protected void CreateNewVisual()
		{
			CurrentStrokeVisual = new RoadStrokeVisual(_strokeInfo);
			AddVisual(CurrentStrokeVisual);
		}

		protected override void OnAppendPoint(StylusPoint point, bool isLastPoint)
		{
			Points.Add(point);
			if (Points.Count < 2)
			{
				_prePoint = (Point)point;
				_drawPoints.Add(point);
				CurrentStrokeVisual.Add(point);
			}
			else
			{
				Vector vector = Point.Subtract((Point)point, _prePoint);
				double length = vector.Length;
				vector.Normalize();
				double num = 10.0 * _strokeInfo.Scale;
				int num2 = (int)(length / num);
				for (int i = 1; i <= num2; i++)
				{
					Point prePoint = _prePoint + vector * num;
					StylusPoint stylusPoint = new StylusPoint(prePoint.X, prePoint.Y, point.PressureFactor);
					if (CurrentStrokeVisual.PointCount == SegmentPointCount)
					{
						CurrentStrokeVisual.Add(stylusPoint);
						CurrentStrokeVisual.Redraw();
						CreateNewVisual();
					}
					_drawPoints.Add(stylusPoint);
					CurrentStrokeVisual.Add(stylusPoint);
					CurrentStrokeVisual.Redraw();
					_prePoint = prePoint;
				}
			}
			base.OnAppendPoint(point, isLastPoint);
		}

		public override Element AsStatic()
		{
			Point topLeft = GetTopLeftPoint();
			StylusPointCollection stylusPointCollection = new StylusPointCollection(_drawPoints.Select((StylusPoint point) => new StylusPoint(point.X - topLeft.X, point.Y - topLeft.Y, point.PressureFactor)));
			RoadStrokeBase roadStrokeBase = (RoadStrokeBase)Activator.CreateInstance(_strokeInfo.Type, stylusPointCollection, _strokeInfo.Scale);
			roadStrokeBase.StrokePointDetailsList = StrokePointDetailsList;
			return new RoadInk(roadStrokeBase, _strokeInfo.StrokeType)
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
			double num = 12.0 * _strokeInfo.Scale;
			return strokeBounds.Expand(num, num, num, num).TopLeft;
		}
	}
}
