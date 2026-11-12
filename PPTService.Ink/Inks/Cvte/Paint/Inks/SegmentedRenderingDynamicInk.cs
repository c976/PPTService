using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using Cvte.Paint.Framework;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public abstract class SegmentedRenderingDynamicInk : DynamicInk
	{
		protected StrokeVisual CurrentStrokeVisual { get; set; }

		public int SegmentPointCount { get; set; }

		protected SegmentedRenderingDynamicInk(UIElement adornedElement, InkBoardSettings state)
			: this(adornedElement, DynamicInk.CreateDrawingAttributes(state))
		{
		}

		protected SegmentedRenderingDynamicInk(UIElement adornedElement, DrawingAttributes drawingAttributes)
			: base(adornedElement, drawingAttributes)
		{
			IsHitTestVisible = false;
			CurrentStrokeVisual = new StrokeVisual(DrawingAttributes);
			AddVisual(CurrentStrokeVisual);
			SegmentPointCount = 500;
		}

		protected virtual void CreateNewVisual()
		{
			CurrentStrokeVisual = new StrokeVisual(DrawingAttributes);
			AddVisual(CurrentStrokeVisual);
		}

		public override Element AsStatic()
		{
			List<Stroke> list = new List<Stroke>();
			for (int i = 0; i < VisualChildrenCount; i++)
			{
				StrokeVisual strokeVisual = (StrokeVisual)GetVisualChild(i);
				if (strokeVisual != null)
				{
					if (IsPostOptimizationEnabled)
					{
						strokeVisual.Stroke.DrawingAttributes.FitToCurve = true;
					}
					list.Add(strokeVisual.Stroke);
				}
			}
			CombinedStroke combinedStroke = new CombinedStroke(list)
			{
				StrokePointDetailsList = StrokePointDetailsList
			};
			if (IsPostOptimizationEnabled)
			{
				combinedStroke.DrawingAttributes.FitToCurve = true;
			}
			return ToInk(combinedStroke);
		}

		protected virtual Element ToInk(CombinedStroke stroke)
		{
			return new Ink(stroke);
		}
	}
}
