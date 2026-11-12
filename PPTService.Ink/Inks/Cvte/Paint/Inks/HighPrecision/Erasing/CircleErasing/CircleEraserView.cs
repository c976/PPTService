using System;
using System.Windows;
using System.Windows.Media;
using Cvte.Windows.Media;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing
{
	internal class CircleEraserView : HighPrecisionEraserViewBase, IEraserView
	{
		private static Lazy<DrawingVisual> LazyCircleEraserDrawingVisual { get; } = new Lazy<DrawingVisual>(CreateCircleEraserDrawingVisual);

		public CircleEraserView()
			: base(LazyCircleEraserDrawingVisual.Value)
		{
		}

		private static DrawingVisual CreateCircleEraserDrawingVisual()
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				DrawingGroup drawingGroup = new DrawingGroup
				{
					Children = 
					{
						(Drawing)new GeometryDrawing
						{
							Brush = BrushCreator.GetOrCreate("#FFF2EEEB"),
							Pen = new Pen(new SolidColorBrush(new Color
							{
								A = byte.MaxValue,
								R = 0,
								B = 0,
								G = 0
							})
							{
								Opacity = 0.15
							}, 1.0)
							{
								StartLineCap = PenLineCap.Flat,
								EndLineCap = PenLineCap.Flat,
								LineJoin = PenLineJoin.Miter
							},
							Geometry = new RectangleGeometry
							{
								Rect = new Rect(0.5, 0.5, 39.0, 39.0),
								RadiusX = 19.5,
								RadiusY = 19.5
							}
						},
						(Drawing)new GeometryDrawing
						{
							Brush = BrushCreator.GetOrCreate("#FFD1CFCD"),
							Geometry = new RectangleGeometry
							{
								Rect = new Rect(12.0, 10.0, 4.0, 20.0),
								RadiusX = 2.0,
								RadiusY = 2.0
							}
						},
						(Drawing)new GeometryDrawing
						{
							Brush = BrushCreator.GetOrCreate("#FFD1CFCD"),
							Geometry = new RectangleGeometry
							{
								Rect = new Rect(24.0, 10.0, 4.0, 20.0),
								RadiusX = 2.0,
								RadiusY = 2.0
							}
						}
					}
				};
				drawingGroup.Freeze();
				drawingContext.DrawDrawing(drawingGroup);
				return drawingVisual;
			}
		}
	}
}
