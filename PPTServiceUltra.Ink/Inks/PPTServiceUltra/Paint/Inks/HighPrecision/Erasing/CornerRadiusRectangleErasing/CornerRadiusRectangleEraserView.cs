using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Paint.Inks.Resources;
using PPTServiceUltra.Ink.Core.Windows.Mathematics.Geometries;

namespace PPTServiceUltra.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	internal class CornerRadiusRectangleEraserView : HighPrecisionEraserViewBase
	{
		public CornerRadiusRectangleEraserView()
			: base(CornerRadiusRectangleEraserViewBuilder.GetEraserDrawingVisual())
		{
		}

		public Geometry GetEraserViewGeometry()
		{
			Rect rect = new Rect(X, Y, Width, Height);
			RectangleGeometry rectangleGeometry = new RectangleGeometry(rect, 5.0, 5.0);
			Point point = rect.Center();
			rectangleGeometry.Transform = new RotateTransform(Rotation, point.X, point.Y);
			return rectangleGeometry;
		}
	}
}
