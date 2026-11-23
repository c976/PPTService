using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Internal.Ink;
using PPTServiceUltra.Paint.Framework;

namespace PPTServiceUltra.Paint.Inks.Legacy
{
	public class GeometryInkHardPenWritingMode : HardPenInputFromBoardWritingMode
	{
		protected override Element StrokeSynchronizerToElement(InkSynchronizer.StrokeSynchronizer synchronizer)
		{
			StylusPointCollection stylusPointCollection = synchronizer.StylusPointCollection;
			DrawingAttributes drawingAttributes = synchronizer.DrawingAttributes;
			Geometry geometry;
			Rect bounds;
			StrokeRenderer.CalcGeometryAndBounds(StrokeNodeIterator.GetIterator(stylusPointCollection, drawingAttributes), drawingAttributes, false, out geometry, out bounds);
			return new GeometryInk(geometry, new SolidColorBrush(drawingAttributes.Color));
		}
	}
}
