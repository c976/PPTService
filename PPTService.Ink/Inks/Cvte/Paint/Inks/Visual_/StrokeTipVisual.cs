using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Internal.Ink;

namespace Cvte.Paint.Inks.Visual_
{
	public class StrokeTipVisual : DrawingVisual, IStrokeTipVisual
	{
		private readonly SolidColorBrush _brush;

		private readonly DrawingAttributes _drawingAttributes;

		public StrokeTipVisual(SolidColorBrush brush, DrawingAttributes drawingAttributes)
		{
			_brush = brush;
			_drawingAttributes = drawingAttributes;
		}

		public void Redraw(StylusPointCollection stylusPoints)
		{
			Geometry geometry;
			Rect bounds;
			StrokeRenderer.CalcGeometryAndBounds(StrokeNodeIterator.GetIterator(stylusPoints, _drawingAttributes), _drawingAttributes, false, out geometry, out bounds);
			using (DrawingContext drawingContext = RenderOpen())
			{
				drawingContext.DrawGeometry(_brush, null, geometry);
			}
		}
	}
}
