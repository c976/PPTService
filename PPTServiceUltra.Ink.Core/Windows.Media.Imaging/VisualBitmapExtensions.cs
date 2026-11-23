using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Windows.Mathematics.Geometries;

namespace PPTServiceUltra.Ink.Core.Windows.Media.Imaging
{
	public static class VisualBitmapExtensions
	{
		private const int DpiX = 96;

		private const int DpiY = 96;

		public static BitmapSource ToImageSource(this Visual visual, Size size, double dpiX, double dpiY)
		{
			return new VisualBrush(visual).ToImageSource(size, dpiX, dpiY);
		}

		public static BitmapSource ToImageSource(this Brush brush, Size size, double dpiX, double dpiY)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				drawingContext.DrawRectangle(brush, null, new Rect(size));
			}
			BitmapImage bitmapImage = new BitmapImage();
			if (Math.Abs(size.Width) > 0.001 && Math.Abs(size.Height) > 0.001)
			{
				RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)(size.Width * dpiX / 96.0), (int)(size.Height * dpiY / 96.0), dpiX, dpiY, PixelFormats.Pbgra32);
				renderTargetBitmap.Render(drawingVisual);
				bitmapImage.BeginInit();
				bitmapImage.DecodePixelWidth = (int)(size.Width * dpiX / 96.0);
				bitmapImage.DecodePixelHeight = (int)(size.Height * dpiY / 96.0);
				bitmapImage.StreamSource = renderTargetBitmap.ToMemoryStream(ImageFormat.Png);
				bitmapImage.EndInit();
				bitmapImage.Freeze();
			}
			return bitmapImage;
		}

		public static BitmapSource ToImageSource([NotNull] this Visual visual)
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(visual);
			if (descendantBounds == Rect.Empty)
			{
				return null;
			}
			return visual.ToImageSource(descendantBounds.Size, Dpi.System.X, Dpi.System.Y);
		}

		public static BitmapSource ToImageSource([NotNull] this Visual visual, Size size)
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			return visual.ToImageSource(size, 96.0, 96.0);
		}

		public static BitmapSource ToImageSource([NotNull] this Visual visual, double maxDimension)
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			if (maxDimension <= 0.0)
			{
				throw new ArgumentOutOfRangeException("maxDimension");
			}
			Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(visual);
			return visual.ToImageSource(((descendantBounds.Height > descendantBounds.Width) ? descendantBounds.UniformSetHeight(maxDimension) : descendantBounds.UniformSetWidth(maxDimension)).Size);
		}
	}
}
