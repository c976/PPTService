using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Windows.Mathematics;
using PPTService.Ink.Core.Windows.Media.Imaging;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows.Media
{
	public static class Snapshot
	{
		public static BitmapSource Take(this Visual element)
		{
			return element.ToImageSource();
		}

		public static BitmapSource TakeWhileNotOnVisualTree(this UIElement element)
		{
			element.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
			element.Arrange(new Rect(default(System.Windows.Point), element.DesiredSize));
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)element.DesiredSize.Width, (int)element.DesiredSize.Height, 96.0, 96.0, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(element);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.DecodePixelWidth = (int)(element.DesiredSize.Width * Dpi.System.FactorX);
			bitmapImage.DecodePixelHeight = (int)(element.DesiredSize.Height * Dpi.System.FactorY);
			bitmapImage.StreamSource = renderTargetBitmap.ToMemoryStream(ImageFormat.Png);
			bitmapImage.EndInit();
			return bitmapImage;
		}

		public static BitmapSource Take(this Visual visual, System.Windows.Size size)
		{
			FrameworkElement frameworkElement = visual as FrameworkElement;
			if (frameworkElement != null)
			{
				if (frameworkElement.IsLoaded)
				{
					return frameworkElement.ToImageSource(size);
				}
				frameworkElement.Measure(size);
				frameworkElement.Arrange(new Rect(size));
			}
			return visual.ToImageSource(size);
		}

		public static BitmapSource Take(double x, double y, double width, double height)
		{
			IntPtr desktopWindow = Win32.GetDesktopWindow();
			IntPtr windowDC = Win32.GetWindowDC(desktopWindow);
			IntPtr intPtr = Win32.CreateCompatibleDC(windowDC);
			IntPtr intPtr2 = Win32.CreateCompatibleBitmap(windowDC, (int)width, (int)height);
			IntPtr hgdiobj = Win32.SelectObject(intPtr, intPtr2);
			Win32.BitBlt(intPtr, 0, 0, (int)width, (int)height, windowDC, (int)x, (int)y, (CopyPixelOperation)1087111200);
			Bitmap bitmap = Image.FromHbitmap(intPtr2);
			Win32.SelectObject(intPtr, hgdiobj);
			Win32.DeleteObject(intPtr2);
			Win32.DeleteDC(intPtr);
			Win32.ReleaseDC(desktopWindow, windowDC);
			return bitmap.ToImageSource();
		}

		public static BitmapSource Take(Rect rect)
		{
			if (Nearly.Equals(rect.Width, 0.0) || Nearly.Equals(rect.Height, 0.0))
			{
				return null;
			}
			return Take(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static BitmapSource Take(Rectangle rectangle)
		{
			return Take(new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
		}

		public static BitmapSource Take(IntPtr hwnd)
		{
			IntPtr windowDC = Win32.GetWindowDC(hwnd);
			IntPtr intPtr = Win32.CreateCompatibleDC(windowDC);
			Win32.RECT lpRect;
			Win32.GetWindowRect(hwnd, out lpRect);
			int num = lpRect.Right - lpRect.Left;
			int num2 = lpRect.Bottom - lpRect.Top;
			IntPtr hgdiobj = Win32.CreateCompatibleBitmap(windowDC, num, num2);
			IntPtr hgdiobj2 = Win32.SelectObject(intPtr, hgdiobj);
			Win32.BitBlt(intPtr, 0, 0, num, num2, windowDC, 0, 0, CopyPixelOperation.SourceCopy);
			hgdiobj = Win32.SelectObject(intPtr, hgdiobj2);
			Bitmap bitmap = Image.FromHbitmap(hgdiobj);
			Win32.DeleteDC(intPtr);
			return bitmap.ToBitmapImage();
		}

		public static BitmapSource Take(Window window)
		{
			return Take(new WindowInteropHelper(window).Handle);
		}

		[CanBeNull]
		public static BitmapSource TakeVirtualScreen()
		{
			try
			{
				return Take(new Rect(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight));
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
