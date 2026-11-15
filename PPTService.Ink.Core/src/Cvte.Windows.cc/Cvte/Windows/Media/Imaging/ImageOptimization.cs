using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Diagnostics.Trace;

namespace PPTService.Ink.Core.Windows.Media.Imaging
{
	public static class ImageOptimization
	{
		private const int MaxWidth = 3840;

		private const int MaxHeight = 2160;

		private const double MinDifference = 0.1;

		private const int DefaultBitsPerPixel = 32;

		private const int MaxExifImageLength = 34603008;

		private const double PixelScaleR = 0.3086;

		private const double PixelScaleG = 0.6094;

		private const double PixelScaleB = 0.082;

		private static ILogTracer Tracer => PPTService.Ink.Core.Diagnostics.Trace.Tracer.BuildLogTracer("ImageOptimization");

		public static string Optimization(string filePath, string desDir, bool copyNewFile = true)
		{
			return Optimization(filePath, desDir, 3840, 2160, copyNewFile);
		}

		public static string Optimization(string filePath, string desDir, int maxImageWidth, int maxImageHeight, bool copyNewFile = true)
		{
			if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(desDir))
			{
				throw new ArgumentException("FilePath or desDir can not be null or empty.");
			}
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(filePath + " dose not exist.");
			}
			if (!Directory.Exists(desDir))
			{
				Directory.CreateDirectory(desDir);
			}
			string extension = Path.GetExtension(filePath);
			if (copyNewFile)
			{
				string text = Path.Combine(desDir, Guid.NewGuid().ToString() + extension);
				File.Copy(filePath, text, true);
				filePath = text;
			}
			string text2 = Path.Combine(desDir, Guid.NewGuid().ToString() + extension);
			if (!string.IsNullOrEmpty(extension))
			{
				switch (extension.ToLower(CultureInfo.InvariantCulture))
				{
				case ".cur":
					CursorExtensions.CurToPng(filePath, text2);
					File.SetAttributes(text2, FileAttributes.Normal);
					return text2;
				case ".gif":
				case ".svg":
					File.Copy(filePath, text2, true);
					File.SetAttributes(text2, FileAttributes.Normal);
					return text2;
				}
			}
			try
			{
				if (new FileInfo(filePath).Length < 34603008)
				{
					using (ExifHelper exifHelper = new ExifHelper(filePath, desDir))
					{
						filePath = exifHelper.GetRotatedImage();
					}
				}
			}
			catch (Exception exception)
			{
				Tracer.Error(exception);
			}
			BitmapImage bitmapImage = CreateImageWithoutColorProfile(new Uri(filePath, UriKind.Relative));
			double num = maxImageWidth * maxImageHeight;
			int pixelWidth;
			int pixelHeight;
			try
			{
				pixelWidth = bitmapImage.PixelWidth;
				pixelHeight = bitmapImage.PixelHeight;
			}
			catch (Exception exception2)
			{
				Tracer.Error(exception2);
				return null;
			}
			if ((double)(pixelWidth * pixelHeight) <= num)
			{
				if (Path.GetDirectoryName(filePath) != Path.GetDirectoryName(text2))
				{
					File.Copy(filePath, text2, true);
					File.SetAttributes(text2, FileAttributes.Normal);
				}
				else
				{
					text2 = filePath;
				}
			}
			else
			{
				BitmapImage bitmapImage2 = new BitmapImage();
				bitmapImage2.BeginInit();
				bitmapImage2.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
				bitmapImage2.DecodePixelWidth = (int)Math.Sqrt(num * (double)pixelWidth / (double)pixelHeight);
				bitmapImage2.DecodePixelHeight = (int)Math.Sqrt(num * (double)pixelHeight / (double)pixelWidth);
				bitmapImage2.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
				bitmapImage2.EndInit();
				if (bitmapImage.CanFreeze)
				{
					bitmapImage.Freeze();
				}
				bitmapImage2.SaveToFile(text2, true);
			}
			return NormalizeImageDpi(bitmapImage, text2, desDir);
		}

		[NotNull]
		public static string NormalizeImagePixelFormat([NotNull] string filePath, [NotNull] string desDir)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			if (desDir == null)
			{
				throw new ArgumentNullException("desDir");
			}
			string text = Path.Combine(desDir, Guid.NewGuid().ToString() + Path.GetExtension(filePath));
			using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (Image image = Image.FromStream(stream, false, false))
				{
					if (Image.GetPixelFormatSize(image.PixelFormat) == 32)
					{
						return filePath;
					}
					using (Bitmap bitmap = CreatePixelFormatBitmap(image, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
					{
						bitmap.Save(text);
						return text;
					}
				}
			}
		}

		private static Bitmap CreatePixelFormatBitmap(Image image, System.Drawing.Imaging.PixelFormat format)
		{
			using (Bitmap bitmap = new Bitmap(image))
			{
				return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), format);
			}
		}

		private static string NormalizeImageDpi(BitmapImage bitmapImage, string filePath, string desDir)
		{
			string text = Path.Combine(desDir, Guid.NewGuid().ToString() + Path.GetExtension(filePath));
			if (!IsNeedChangeImageDpi(bitmapImage, Dpi.ScreenStandard))
			{
				return filePath;
			}
			ChangeImageDpi(filePath, text, Dpi.ScreenStandard);
			return text;
		}

		public static void ChangeImageDpi(string sourceFile, string destinationDirectory, out string targetFile, Dpi dpiChangingTo)
		{
			targetFile = Path.Combine(destinationDirectory, Guid.NewGuid().ToString() + Path.GetExtension(sourceFile));
			ChangeImageDpi(sourceFile, targetFile, dpiChangingTo);
		}

		private static void ChangeImageDpi(string sourceFile, string targetFile, Dpi dpiChangingTo)
		{
			using (FileStream stream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (Image image = Image.FromStream(stream, false, false))
				{
					if (Image.GetPixelFormatSize(image.PixelFormat) > 32)
					{
						using (Bitmap bitmap = CreatePixelFormatBitmap(image, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
						{
							bitmap.SetResolution(dpiChangingTo.X, dpiChangingTo.Y);
							bitmap.Save(targetFile);
							return;
						}
					}
					using (Bitmap bitmap2 = new Bitmap(image))
					{
						bitmap2.SetResolution(dpiChangingTo.X, dpiChangingTo.Y);
						bitmap2.Save(targetFile, image.RawFormat);
					}
				}
			}
		}

		public static bool IsNeedChangeImageDpi(BitmapSource bitmap, Dpi dpiChangingTo)
		{
			if (!(Math.Abs(bitmap.DpiX - (double)dpiChangingTo.X) > 0.1))
			{
				return Math.Abs(bitmap.DpiY - (double)dpiChangingTo.Y) > 0.1;
			}
			return true;
		}

		public static string AdjustBrightnessContrastSaturation(Uri originalFilePath, Uri currentFilePath, string desDir, double brightness, double contrast, double saturation)
		{
			if (!Directory.Exists(desDir))
			{
				Directory.CreateDirectory(desDir);
			}
			string extension = Path.GetExtension(currentFilePath.OriginalString);
			string text = Path.Combine(desDir, Guid.NewGuid().ToString() + extension);
			BitmapImage bitmapImage = new BitmapImage(originalFilePath);
			BitmapSource currentBitmapSource = new BitmapImage(currentFilePath);
			int bitsPerPixel = bitmapImage.Format.BitsPerPixel;
			double dpiX = bitmapImage.DpiX;
			double dpiY = bitmapImage.DpiY;
			int pixelWidth = bitmapImage.PixelWidth;
			int pixelHeight = bitmapImage.PixelHeight;
			int stride = (pixelWidth * bitsPerPixel + 7) / 8;
			byte[] originalPixels = GetOriginalPixels(bitmapImage, currentBitmapSource, brightness, contrast, saturation);
			BitmapSource.Create(pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Bgra32, null, originalPixels, stride).SaveToFile(text);
			return text;
		}

		private static byte[] GetOriginalPixels(BitmapSource originalBitmapSource, BitmapSource currentBitmapSource, double brightness, double contrast, double saturation)
		{
			int bitsPerPixel = originalBitmapSource.Format.BitsPerPixel;
			int pixelWidth = originalBitmapSource.PixelWidth;
			int height = originalBitmapSource.PixelHeight;
			int stride = (pixelWidth * bitsPerPixel + 7) / 8;
			brightness /= 500.0;
			contrast /= 1300.0;
			byte bitmapPixelFormatSize = (byte)(bitsPerPixel / 8);
			byte[] originalPixels = new byte[stride * height];
			byte[] currentPixels = new byte[stride * height];
			originalBitmapSource.CopyPixels(Int32Rect.Empty, originalPixels, stride, 0);
			currentBitmapSource.CopyPixels(Int32Rect.Empty, currentPixels, stride, 0);
			Parallel.For(0, pixelWidth, (int i) =>
			{
				Parallel.For(0, height, (int j) =>
				{
					int num = i * bitmapPixelFormatSize + j * stride;
					for (int k = 0; k < 4; k++)
					{
						if (k < 3)
						{
							SetPixelBrightness(originalPixels, num + k, brightness);
						}
						SetPixelContrast(originalPixels, num + k, contrast);
					}
					SetPixelRgbValue(originalPixels, num, saturation);
					originalPixels[num + 3] = currentPixels[num + 3];
				});
			});
			return originalPixels;
		}

		private static void SetPixelContrast(byte[] originalPixels, int index, double contrast)
		{
			double num = (double)(int)originalPixels[index] / 255.0;
			int i = (int)((num - contrast * (num - 1.0) * num * (num - 0.5) * 10.0) * 255.0);
			originalPixels[index] = ByteRange(i);
		}

		private static void SetPixelBrightness(byte[] originalPixels, int index, double brightness)
		{
			int i = (int)(((double)(int)originalPixels[index] / 255.0 + brightness) * 255.0);
			originalPixels[index] = ByteRange(i);
		}

		private static void SetPixelRgbValue(byte[] originalPixels, int index, double saturation)
		{
			saturation /= 500.0;
			double num = 1.0 - saturation * 0.3086 + saturation;
			double num2 = (0.0 - saturation) * 0.3086;
			double num3 = (0.0 - saturation) * 0.3086;
			double num4 = (0.0 - saturation) * 0.6094;
			double num5 = 1.0 - saturation * 0.6094 + saturation;
			double num6 = (0.0 - saturation) * 0.6094;
			double num7 = (0.0 - saturation) * 0.082;
			double num8 = (0.0 - saturation) * 0.082;
			double num9 = 1.0 - saturation * 0.082 + saturation;
			double num10 = (double)(int)originalPixels[index] / 255.0;
			double num11 = (double)(int)originalPixels[index + 1] / 255.0;
			double num12 = (double)(int)originalPixels[index + 2] / 255.0;
			int i = (int)((num10 * num + num11 * num4 + num12 * num7) * 2550.0) / 10;
			int i2 = (int)((num10 * num2 + num11 * num5 + num12 * num8) * 2550.0) / 10;
			int i3 = (int)((num10 * num3 + num11 * num6 + num12 * num9) * 2550.0) / 10;
			originalPixels[index] = ByteRange(i);
			originalPixels[index + 1] = ByteRange(i2);
			originalPixels[index + 2] = ByteRange(i3);
		}

		public static string AdjustAlpha(Uri originalFilePath, string desDir, double alpha)
		{
			if (alpha <= 0.0 || alpha > 1.0)
			{
				throw new ArgumentOutOfRangeException("alpha");
			}
			if (!Directory.Exists(desDir))
			{
				Directory.CreateDirectory(desDir);
			}
			string text = Path.Combine(desDir, Guid.NewGuid().ToString() + ".png");
			BitmapSource bitmapSource = new BitmapImage(originalFilePath);
			int bitsPerPixel = bitmapSource.Format.BitsPerPixel;
			if (bitsPerPixel != 32)
			{
				throw new FormatException("BitsPerPixel is not 32");
			}
			double dpiX = bitmapSource.DpiX;
			double dpiY = bitmapSource.DpiY;
			int pixelWidth = bitmapSource.PixelWidth;
			int height = bitmapSource.PixelHeight;
			int stride = (pixelWidth * bitsPerPixel + 7) / 8;
			byte bitmapPixelFormatSize = (byte)(bitsPerPixel / 8);
			byte[] originalPixels = new byte[stride * height];
			bitmapSource.CopyPixels(Int32Rect.Empty, originalPixels, stride, 0);
			Parallel.For(0, pixelWidth, (int i) =>
			{
				Parallel.For(0, height, (int j) =>
				{
					int num = i * bitmapPixelFormatSize + j * stride;
					originalPixels[num + 3] = (byte)(255.0 * alpha);
				});
			});
			BitmapSource.Create(pixelWidth, height, dpiX, dpiY, PixelFormats.Bgra32, null, originalPixels, stride).SaveToFile(text);
			return text;
		}

		public static string SetImageBackgroundWhite(string filePath, string desDir)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(filePath))
				{
					return null;
				}
				if (!Directory.Exists(desDir))
				{
					Directory.CreateDirectory(desDir);
				}
				string extension = Path.GetExtension(filePath);
				string text = Path.Combine(desDir, Guid.NewGuid().ToString() + extension);
				using (Image image = Image.FromFile(filePath))
				{
					using (Graphics graphics = Graphics.FromImage(image))
					{
						System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.White, image.Width);
						graphics.DrawRectangle(pen, 0, 0, image.Width, image.Height);
						Image image2 = Image.FromFile(filePath);
						graphics.DrawImage(image2, 0, 0, image2.Width, image2.Height);
						image.Save(text);
					}
				}
				return text;
			}
			catch (Exception exception)
			{
				Tracer.Error(exception);
				return null;
			}
		}

		public static string ImageHorizontalReversal([NotNull] string filePath, string desDir)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			if (!File.Exists(filePath))
			{
				Tracer.Info(filePath + "does not exist.");
				return null;
			}
			if (!Directory.Exists(desDir))
			{
				Directory.CreateDirectory(desDir);
			}
			string extension = Path.GetExtension(filePath);
			string text = Path.Combine(desDir, Guid.NewGuid().ToString() + extension);
			BitmapSource bitmapSource = new BitmapImage(new Uri(filePath));
			int bitsPerPixel = bitmapSource.Format.BitsPerPixel;
			double dpiX = bitmapSource.DpiX;
			double dpiY = bitmapSource.DpiY;
			int width = bitmapSource.PixelWidth;
			int pixelHeight = bitmapSource.PixelHeight;
			int stride = (width * bitsPerPixel + 7) / 8;
			byte bitmapPixelFormatSize = (byte)(bitsPerPixel / 8);
			byte[] originalPixels = new byte[stride * pixelHeight];
			bitmapSource.CopyPixels(Int32Rect.Empty, originalPixels, stride, 0);
			Parallel.For(0, pixelHeight, (int i) =>
			{
				Parallel.For(0, width / 2, (int j) =>
				{
					int num = i * stride + j * bitmapPixelFormatSize;
					int num2 = i * stride + (width - j - 1) * bitmapPixelFormatSize;
					for (int k = 0; k <= 3; k++)
					{
						int num3 = num + k;
						int num4 = num2 + k;
						if (originalPixels[num3] != originalPixels[num4])
						{
							byte b = originalPixels[num3];
							originalPixels[num3] = originalPixels[num4];
							originalPixels[num4] = b;
						}
					}
				});
			});
			BitmapSource.Create(width, pixelHeight, dpiX, dpiY, PixelFormats.Bgra32, null, originalPixels, stride).SaveToFile(text);
			return text;
		}

		public static void CompressImage(string filePath, string desPath, byte compressionLevel = 2)
		{
			if (!File.Exists(filePath))
			{
				return;
			}
			try
			{
				using (Bitmap bitmap = new Bitmap(filePath))
				{
					CompressImage(bitmap, desPath, compressionLevel);
				}
			}
			catch (Exception exception)
			{
				Tracer.Error(exception);
			}
		}

		public static void CompressImage(Bitmap bitmap, string desPath, long compressionLevel = 75L)
		{
			try
			{
				using (EncoderParameters encoderParameters = new EncoderParameters(1))
				{
					encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, compressionLevel);
					ImageCodecInfo encoderInfo = GetEncoderInfo(System.Drawing.Imaging.ImageFormat.Jpeg);
					if (encoderInfo == null)
					{
						Tracer.Error("ImageCodecInfo is null.");
					}
					else
					{
						bitmap.Save(desPath, encoderInfo, encoderParameters);
					}
				}
			}
			catch (Exception exception)
			{
				Tracer.Error(exception);
			}
		}

		private static ImageCodecInfo GetEncoderInfo(System.Drawing.Imaging.ImageFormat format)
		{
			ImageCodecInfo[] imageDecoders = ImageCodecInfo.GetImageDecoders();
			foreach (ImageCodecInfo imageCodecInfo in imageDecoders)
			{
				if (imageCodecInfo.FormatID == format.Guid)
				{
					return imageCodecInfo;
				}
			}
			return null;
		}

		private static byte ByteRange(int i)
		{
			if (i < 0)
			{
				return 0;
			}
			if (i > 255)
			{
				return byte.MaxValue;
			}
			return (byte)i;
		}

		public static BitmapImage CreateImageWithoutColorProfile(Uri uri)
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
			if ((object)uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			bitmapImage.UriSource = uri;
			bitmapImage.EndInit();
			return bitmapImage;
		}
	}
}
