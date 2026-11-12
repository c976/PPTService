using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks.Resources
{
	internal static class CornerRadiusRectangleEraserViewBuilder
	{
		private static DrawingVisual DrawingVisual { get; set; }

		public static Drawing CreateEraserDrawing()
		{
			GeometryDrawing value = new GeometryDrawing
			{
				Geometry = Geometry.Parse("M0,5.0093855C0,2.24277828,2.2303666,0,5.00443555,0L24.9955644,0C27.7594379,0,30,2.23861485,30,4.99982044L30,17.9121669C30,20.6734914,30,25.1514578,30,27.9102984L30,40.0016889C30,42.7621799,27.7696334,45,24.9955644,45L5.00443555,45C2.24056212,45,0,42.768443,0,39.9906145L0,5.0093855z"),
				Brush = new SolidColorBrush(Color.FromArgb(51, 0, 0, 0))
			};
			GeometryDrawing value2 = new GeometryDrawing
			{
				Geometry = new RectangleGeometry(new Rect(1.0, 1.0, 28.0, 43.0), 4.0, 4.0),
				Brush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 242, 238, 235))
			};
			GeometryDrawing value3 = new GeometryDrawing
			{
				Geometry = Geometry.Parse("M1,5.00672201C1,2.79387055,2.79412025,1,4.99683887,1L25.0031611,1C27.2105543,1,29,2.79291122,29,5.00672201L29,39.993278C29,42.2061295,27.2058797,44,25.0031611,44L4.99683887,44C2.78944571,44,1,42.2070888,1,39.993278L1,5.00672201z M1.66666667,5.16533587C1.66666667,3.23307422,3.23523169,1.66666667,5.16656805,1.66666667L24.8334319,1.66666667C26.7663741,1.66666667,28.3333333,3.22400386,28.3333333,5.16533587L28.3333333,39.8346641C28.3333333,41.7669258,26.7647683,43.3333333,24.8334319,43.3333333L5.16656805,43.3333333C3.23362589,43.3333333,1.66666667,41.7759961,1.66666667,39.8346641L1.66666667,5.16533587z"),
				Brush = new SolidColorBrush(Color.FromArgb(76, byte.MaxValue, byte.MaxValue, byte.MaxValue))
			};
			GeometryDrawing value4 = new GeometryDrawing
			{
				Geometry = Geometry.Parse("M21,29.1666667L21,16.1666667C21,14.7859548 19.8807119,13.6666667 18.5,13.6666667 17.1192881,13.6666667 16,14.7859548 16,16.1666667L16,29.1666667C16,30.5473785 17.1192881,31.6666667 18.5,31.6666667 19.8807119,31.6666667 21,30.5473785 21,29.1666667z M14,29.1666667L14,16.1666667C14,14.7859548 12.8807119,13.6666667 11.5,13.6666667 10.1192881,13.6666667 9,14.7859548 9,16.1666667L9,29.1666667C9,30.5473785 10.1192881,31.6666667 11.5,31.6666667 12.8807119,31.6666667 14,30.5473785 14,29.1666667z"),
				Brush = new SolidColorBrush(Color.FromArgb(76, byte.MaxValue, byte.MaxValue, byte.MaxValue))
			};
			GeometryDrawing value5 = new GeometryDrawing
			{
				Geometry = Geometry.Parse("M20,29.1666667L20,16.1666667C20,15.3382395 19.3284271,14.6666667 18.5,14.6666667 17.6715729,14.6666667 17,15.3382395 17,16.1666667L17,29.1666667C17,29.9950938 17.6715729,30.6666667 18.5,30.6666667 19.3284271,30.6666667 20,29.9950938 20,29.1666667z M13,29.1666667L13,16.1666667C13,15.3382395 12.3284271,14.6666667 11.5,14.6666667 10.6715729,14.6666667 10,15.3382395 10,16.1666667L10,29.1666667C10,29.9950938 10.6715729,30.6666667 11.5,30.6666667 12.3284271,30.6666667 13,29.9950938 13,29.1666667z"),
				Brush = new SolidColorBrush(Color.FromArgb(38, 0, 0, 0))
			};
			return new DrawingGroup
			{
				Children = new DrawingCollection { value, value2, value3, value4, value5 }
			};
		}

		public static DrawingVisual GetEraserDrawingVisual()
		{
			if (DrawingVisual == null)
			{
				DrawingVisual drawingVisual = new DrawingVisual();
				using (DrawingContext drawingContext = drawingVisual.RenderOpen())
				{
					drawingContext.DrawDrawing(CreateEraserDrawing());
				}
				DrawingVisual = drawingVisual;
			}
			return DrawingVisual;
		}
	}
}
