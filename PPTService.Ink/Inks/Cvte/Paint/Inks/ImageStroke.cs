using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cvte.Paint.Inks
{
	public class ImageStroke : StrokeBase
	{
		internal BitmapImage InkImage { get; }

		public double BitmapScale { get; set; }

		public ImageStroke(StylusPointCollection stylusPoints, BitmapImage inkImage, double bitmapScale)
		{
			InkImage = inkImage;
			StylusPoints = stylusPoints;
			BitmapScale = bitmapScale;
			DrawingAttributes.Width = (double)inkImage.PixelWidth * bitmapScale;
			DrawingAttributes.Height = (double)inkImage.PixelHeight * bitmapScale;
		}

		protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
		{
			if (InkImage.CanFreeze)
			{
				InkImage.Freeze();
			}
			double width = drawingAttributes.Width;
			double height = drawingAttributes.Height;
			foreach (StylusPoint stylusPoint in StylusPoints)
			{
				drawingContext.DrawImage(rectangle: new Rect(stylusPoint.X - width / 2.0, stylusPoint.Y - height / 2.0, width, height), imageSource: InkImage);
			}
		}
	}
}
