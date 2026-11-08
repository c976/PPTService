using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PPTService
{
	public static class Snapshot
	{
		public static BitmapSource Take(double x, double y, double width, double height)
		{
			IntPtr desktopWindow = WindowsApi.GetDesktopWindow();
			IntPtr windowDC = WindowsApi.GetWindowDC(desktopWindow);
			IntPtr intPtr = WindowsApi.CreateCompatibleDC(windowDC);
			IntPtr intPtr2 = WindowsApi.CreateCompatibleBitmap(windowDC, (int)width, (int)height);
			IntPtr hgdiobj = WindowsApi.SelectObject(intPtr, intPtr2);
			WindowsApi.BitBlt(intPtr, 0, 0, (int)width, (int)height, windowDC, (int)x, (int)y, (CopyPixelOperation)1087111200);
			Bitmap bitmap = Image.FromHbitmap(intPtr2);
			WindowsApi.SelectObject(intPtr, hgdiobj);
			WindowsApi.DeleteObject(intPtr2);
			WindowsApi.DeleteDC(intPtr);
			WindowsApi.ReleaseDC(desktopWindow, windowDC);
			IntPtr hbitmap = bitmap.GetHbitmap();
			BitmapSource result = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			if (!WindowsApi.DeleteObject(hbitmap))
			{
				throw new Win32Exception("gdi32.dll error occurred when deleting Hbitmap object.");
			}
			return result;
		}

		public static BitmapSource Take(Rect rect)
		{
			if (Math.Abs(rect.Width) < 1.0 || Math.Abs(rect.Height) < 1.0)
			{
				return null;
			}
			return Take(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static BitmapSource Take(Rectangle rectangle)
		{
			return Take(new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
		}
	}
}
