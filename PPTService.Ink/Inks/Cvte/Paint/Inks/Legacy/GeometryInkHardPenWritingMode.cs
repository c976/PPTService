using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Internal.Ink;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks.Legacy
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
