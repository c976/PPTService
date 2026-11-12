using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks.Resources
{
	public class EraserView : FrameworkElement
	{
		protected override int VisualChildrenCount { get; }

		protected static DrawingVisual DrawingVisual => CornerRadiusRectangleEraserViewBuilder.GetEraserDrawingVisual();

		protected override void OnRender(DrawingContext drawingContext)
		{
			Rect descendantBounds = DrawingVisual.DescendantBounds;
			double width = Width;
			double height = Height;
			ScaleTransform scaleTransform = new ScaleTransform();
			if (double.IsNaN(width) || double.IsNaN(height))
			{
				scaleTransform.ScaleX = 1.0;
				scaleTransform.ScaleY = 1.0;
			}
			else
			{
				scaleTransform.ScaleX = width / descendantBounds.Width;
				scaleTransform.ScaleY = height / descendantBounds.Height;
			}
			drawingContext.PushTransform(scaleTransform);
			drawingContext.DrawDrawing(DrawingVisual.Drawing);
			drawingContext.Pop();
		}

		protected override Visual GetVisualChild(int index)
		{
			return DrawingVisual;
		}
	}
}
