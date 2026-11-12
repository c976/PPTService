using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class HardPenDynamicInk : SegmentedRenderingDynamicInk
	{
		public HardPenDynamicInk(UIElement adornedElement, InkBoardSettings state)
			: this(adornedElement, DynamicInk.CreateDrawingAttributes(state))
		{
		}

		public HardPenDynamicInk(UIElement adornedElement, DrawingAttributes drawingAttributes)
			: base(adornedElement, drawingAttributes)
		{
			SegmentPointCount = 20;
		}

		protected override void OnAppendPoint(StylusPoint point, bool isLastPoint)
		{
			if (CurrentStrokeVisual.PointCount == SegmentPointCount)
			{
				CurrentStrokeVisual.Add(point);
				CurrentStrokeVisual.Redraw();
				CreateNewVisual();
			}
			Points.Add(point);
			CurrentStrokeVisual.Add(point);
			if (isLastPoint)
			{
				CurrentStrokeVisual.Redraw();
			}
			base.OnAppendPoint(point, isLastPoint);
		}
	}
}
