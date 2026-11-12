using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Internal.Ink;

namespace Cvte.Paint.Inks.Visual_
{
	public class StrokeVisual : DrawingVisual
	{
		private readonly SolidColorBrush _brush;

		private readonly DrawingAttributes _drawingAttributes;

		public Stroke Stroke { get; set; }

		protected bool ShouldUseCustomRedraw { get; set; }

		public StrokeVisual(SolidColorBrush brush, DrawingAttributes drawingAttributes)
		{
			_brush = brush;
			_drawingAttributes = drawingAttributes;
		}

		public StrokeVisual(SolidColorBrush brush, DrawingAttributes drawingAttributes, StylusPointCollection stylusPointCollection)
			: this(brush, drawingAttributes)
		{
			Stroke = new Stroke(stylusPointCollection)
			{
				DrawingAttributes = _drawingAttributes
			};
		}

		public void Redraw()
		{
			if (ShouldUseCustomRedraw)
			{
				CustomRedraw();
				return;
			}
			using (DrawingContext drawingContext = RenderOpen())
			{
				Geometry geometry;
				Rect bounds;
				StrokeRenderer.CalcGeometryAndBounds(StrokeNodeIterator.GetIterator(Stroke, _drawingAttributes), _drawingAttributes, false, out geometry, out bounds);
				drawingContext.DrawGeometry(_brush, null, geometry);
			}
		}

		protected virtual void CustomRedraw()
		{
		}
	}
}
