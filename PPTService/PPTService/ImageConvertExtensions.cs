using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace PPTService
{
	public static class ImageConvertExtensions
	{
		public static void SaveToFile(this BitmapSource bitmap, ImageFormat format, string fileName, bool overwrite = false)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			if (File.Exists(fileName) && !overwrite)
			{
				throw new IOException("File " + fileName + " already Exists.");
			}
			BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
			bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
			using (Stream stream = File.Create(fileName))
			{
				bitmapEncoder.Save(stream);
			}
		}

		public static void SaveToFile(this BitmapSource bitmap, string fileName, bool overwrite = false)
		{
			ImageFormat png = ImageFormat.Png;
			bitmap.SaveToFile(png, fileName, overwrite);
		}
	}
}
