using System;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Cvte.Paint.Inks.BrushPen_
{
	[Obsolete("En3的软笔，没有完全实现")]
	internal class BrushStrokePenDynamicRenderer : DynamicRenderer
	{
		private int _n;

		protected StylusPointCollection InkPoints { get; private set; }

		public BrushStrokeOptimizer Optimizer { get; set; }

		public BrushStrokePenDynamicRenderer(InkCanvas inkCanvas)
			: base(inkCanvas)
		{
		}

		protected override void OnDown(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			InkPoints = new StylusPointCollection(stylusPointCollectionInfo.StylusPlugInCollection.Description);
			foreach (StylusPoint item in stylusPointCollectionInfo.StylusPlugInCollection)
			{
				OnAppendInkPoint(item, stylusPointCollectionInfo.StrokeInfo.FillSolidColorBrush, stylusPointCollectionInfo.StrokeInfo.DrawingAttributes);
			}
		}

		protected override void OnMove(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			foreach (StylusPoint item in stylusPointCollectionInfo.StylusPlugInCollection)
			{
				_n++;
				if (_n == 5)
				{
					_n = 0;
				}
				else
				{
					OnAppendInkPoint(item, stylusPointCollectionInfo.StrokeInfo.FillSolidColorBrush, stylusPointCollectionInfo.StrokeInfo.DrawingAttributes);
				}
			}
		}

		protected override void OnUp(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			foreach (StylusPoint item in stylusPointCollectionInfo.StylusPlugInCollection)
			{
				OnAppendInkPoint(item, stylusPointCollectionInfo.StrokeInfo.FillSolidColorBrush, stylusPointCollectionInfo.StrokeInfo.DrawingAttributes);
			}
			Optimizer = null;
		}

		protected void OnAppendInkPoint(StylusPoint point, SolidColorBrush strokeInfoFillSolidColorBrush, DrawingAttributes strokeInfoDrawingAttributes)
		{
			point.PressureFactor = PressureFactor.Current;
			InkPoints.Add(point);
			if (InkPoints.Count >= 2)
			{
				if (Optimizer == null)
				{
					Stroke stroke = new Stroke(new StylusPointCollection(InkPoints))
					{
						DrawingAttributes = strokeInfoDrawingAttributes
					};
					Optimizer = new BrushStrokeOptimizer(stroke);
					return;
				}
				DrawingVisual drawingVisual = new DrawingVisual();
				DrawingContext drawingContext = drawingVisual.RenderOpen();
				int count = Optimizer.Stroke.StylusPoints.Count;
				Optimizer.Add(new StylusPoint(point.X, point.Y));
				Geometry geometry = new Stroke(new StylusPointCollection(Optimizer.Stroke.StylusPoints.Skip(count - 1)), Optimizer.Stroke.DrawingAttributes).GetGeometry();
				drawingContext.DrawGeometry(strokeInfoFillSolidColorBrush, null, geometry);
				drawingContext.Close();
				AddVisual(drawingVisual);
			}
		}

		private void AddVisual(DrawingVisual visual)
		{
			InkCanvas.InkFromTouchContainerVisual.Children.Add(visual);
		}
	}
}
