using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows.Media.Imaging
{
	public static class ImageConvertExtensions
	{
		public static BitmapSource ToImageSource(this Bitmap bitmap)
		{
			IntPtr hbitmap = bitmap.GetHbitmap();
			BitmapSource result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			if (!Win32.Gdi32.DeleteObject(hbitmap))
			{
				throw new Win32Exception("gdi32.dll error occurred when deleting Hbitmap object.");
			}
			return result;
		}

		public static MemoryStream ToMemoryStream(this BitmapSource bitmap, ImageFormat format)
		{
			MemoryStream memoryStream = new MemoryStream();
			BitmapEncoder bitmapEncoder = format.CreateEncoder();
			if (bitmapEncoder == null)
			{
				throw new NotSupportedException(format.Name + " format is not supported.");
			}
			bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
			bitmapEncoder.Save(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		}

		public static Bitmap ToBitmap(this BitmapSource bitmap, ImageFormat format)
		{
			return new Bitmap(bitmap.ToMemoryStream(format));
		}

		public static Bitmap ToBitmap(this BitmapSource bitmap)
		{
			MemoryStream memoryStream = new MemoryStream();
			PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
			pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
			pngBitmapEncoder.Save(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return new Bitmap(memoryStream);
		}

		public static BitmapImage ToBitmapImage(this Bitmap bitmap, bool bitmapDispose = true)
		{
			MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
			bitmapImage.EndInit();
			memoryStream.Dispose();
			if (bitmapDispose)
			{
				bitmap.Dispose();
			}
			return bitmapImage;
		}

		public static void SaveToFile([NotNull] this BitmapSource bitmap, ImageFormat format, string fileName, bool overwrite = false)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			if (File.Exists(fileName) && !overwrite)
			{
				throw new IOException("File " + fileName + " already Exists.");
			}
			BitmapEncoder bitmapEncoder = format.CreateEncoder();
			if (bitmapEncoder == null)
			{
				throw new NotSupportedException(format.Name + " format is not supported.");
			}
			BitmapEncoder bitmapEncoder2 = bitmapEncoder;
			if (bitmapEncoder2.Frames == null)
			{
				IList<BitmapFrame> list = (bitmapEncoder2.Frames = new List<BitmapFrame>());
			}
			bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
			using (Stream stream = File.Create(fileName))
			{
				bitmapEncoder.Save(stream);
			}
		}

		public static void SaveToFile(this BitmapSource bitmap, string fileName, bool overwrite = false)
		{
			ImageFormat imageFormat = ImageFormat.FromFileName(fileName);
			bitmap.SaveToFile(imageFormat ?? ImageFormat.Png, fileName, overwrite);
		}

		public static void ConvertFormat(string sourceFileName, string targetFileName)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (FileStream fileStream = File.Open(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					fileStream.CopyTo(memoryStream);
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.StreamSource = memoryStream;
					bitmapImage.EndInit();
					bitmapImage.SaveToFile(targetFileName, true);
				}
			}
		}
	}
}
