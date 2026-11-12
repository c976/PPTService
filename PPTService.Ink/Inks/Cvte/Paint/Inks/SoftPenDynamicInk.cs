using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class SoftPenDynamicInk : SegmentedRenderingDynamicInk
	{
		public SoftPenStrokeBuilder SoftPenStrokeBuilder { get; private set; }

		public SoftPenDynamicInk(UIElement adornedElement, InkBoardSettings state)
			: this(adornedElement, new DrawingAttributes
			{
				Color = state.InkColor,
				Width = state.InkThickness * state.InkScale,
				Height = state.InkThickness * state.InkScale,
				FitToCurve = false
			})
		{
		}

		public SoftPenDynamicInk(UIElement adornedElement, DrawingAttributes drawingAttributes)
			: base(adornedElement, drawingAttributes)
		{
			IsHitTestVisible = false;
			SegmentPointCount = 20;
			drawingAttributes.FitToCurve = false;
			Matrix stylusTipTransform = default(Matrix);
			stylusTipTransform.Scale(0.6, 1.0);
			stylusTipTransform.Rotate(-52.5);
			drawingAttributes.StylusTipTransform = stylusTipTransform;
			SoftPenStrokeBuilder = new SoftPenStrokeBuilder
			{
				StrokeThickness = drawingAttributes.Width,
				StrokeColor = drawingAttributes.Color,
				DrawingAttributes = drawingAttributes
			};
		}

		protected override void OnAppendPoint(StylusPoint point, bool isLastPoint)
		{
			if (CurrentStrokeVisual.PointCount == SegmentPointCount + 8)
			{
				CreateNewVisual();
			}
			Points.Add(point);
			CurrentStrokeVisual.Add(point);
			SoftPenStrokeBuilder.FitToCurve = !IsBezierInterpolationEnabled;
			SoftPenStrokeBuilder.AddPoint(point);
			CurrentStrokeVisual.SyncPoints(SoftPenStrokeBuilder.BuildingStroke, 8, 0);
			if (isLastPoint)
			{
				CurrentStrokeVisual.Redraw();
			}
			base.OnAppendPoint(point, isLastPoint);
		}

		protected override void CreateNewVisual()
		{
			Stroke stroke = new Stroke(new StylusPointCollection(CurrentStrokeVisual.Stroke.StylusPoints.Skip(SegmentPointCount - 1)), DrawingAttributes);
			for (int i = 0; i < 8; i++)
			{
				CurrentStrokeVisual.Stroke.StylusPoints.RemoveAt(CurrentStrokeVisual.Stroke.StylusPoints.Count - 1);
			}
			CurrentStrokeVisual.Redraw();
			CurrentStrokeVisual = new StrokeVisual(DrawingAttributes)
			{
				Stroke = stroke
			};
			AddVisual(CurrentStrokeVisual);
			CurrentStrokeVisual.Redraw();
		}
	}
}
