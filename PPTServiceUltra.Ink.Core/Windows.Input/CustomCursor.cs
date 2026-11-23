using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public static class CustomCursor
	{
		public static Cursor Create(UIElement element, int xHotSpot = 0, int yHotSpot = 0)
		{
			element.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
			element.Arrange(new Rect(default(System.Windows.Point), element.DesiredSize));
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)element.DesiredSize.Width, (int)element.DesiredSize.Height, 96.0, 96.0, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(element);
			PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
			pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
			using (MemoryStream stream = new MemoryStream())
			{
				pngBitmapEncoder.Save(stream);
				using (Bitmap bmp = new Bitmap(stream))
				{
					return CreateCursorInternal(bmp, xHotSpot, yHotSpot);
				}
			}
		}

		private static Cursor CreateCursorInternal(Bitmap bmp, int xHotSpot, int yHotSpot)
		{
			Win32.IconInfo pIconInfo = default(Win32.IconInfo);
			Win32.GetIconInfo(bmp.GetHicon(), ref pIconInfo);
			pIconInfo.xHotspot = xHotSpot;
			pIconInfo.yHotspot = yHotSpot;
			pIconInfo.fIcon = false;
			return CursorInteropHelper.Create(Win32.CreateIconIndirect(ref pIconInfo));
		}
	}
}
