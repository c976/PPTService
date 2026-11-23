using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	internal class BitmapCachePool
	{
		internal delegate bool WhenForceNaturalCallback(Size naturalSize, Size actualSize);

		private readonly ConcurrentDictionary<ValueTuple<string, int, int>, WeakReference<BitmapSource>> _bitmapPool = new ConcurrentDictionary<ValueTuple<string, int, int>, WeakReference<BitmapSource>>();

		private readonly ConcurrentDictionary<string, Size> _naturalSizeCache = new ConcurrentDictionary<string, Size>(StringComparer.OrdinalIgnoreCase);

		internal Size GetImageSize(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException("文件不存在，无法解析其原始尺寸。", fileName);
			}
			BitmapFrame bitmapFrame = BitmapDecoder.Create(new Uri(fileName, UriKind.Absolute), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None).Frames[0];
			return new Size(bitmapFrame.PixelWidth, bitmapFrame.PixelHeight);
		}

		
		internal ValueTuple<BitmapSource, Size, Size> GetImage(FileSystemInfo file, Size limitedSize, Rect viewbox, WhenForceNaturalCallback forceNatural = null)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			string fullName = file.FullName;
			Size size = GetImageSizeFromCache(fullName) ?? GetImageSize(fullName);
			Size size2 = limitedSize.ToScaledSize(1.0 / viewbox.Width, 1.0 / viewbox.Height);
			size2 = new Size(Math.Min(size2.Width, size.Width), Math.Min(size2.Height, size.Height));
			if (forceNatural != null && forceNatural(size, size2))
			{
				size2 = size;
			}
			int num = (int)Math.Round(size2.Width);
			int num2 = (int)Math.Round(size2.Height);
			int val = Math.Min(10, num);
			int val2 = Math.Min(10, num2);
			val = Math.Max(val, 1);
			val2 = Math.Max(val2, 1);
			num = Math.Max(num, val);
			num2 = Math.Max(num2, val2);
			BitmapSource target = null;
			WeakReference<BitmapSource> value;
			int num3;
			object obj;
			if (_bitmapPool.TryGetValue(new ValueTuple<string, int, int>(fullName, num, num2), out value))
			{
				num3 = (value.TryGetTarget(out target) ? 1 : 0);
				if (num3 != 0)
				{
					obj = target;
					goto IL_0131;
				}
			}
			else
			{
				num3 = 0;
			}
			obj = CreateImage(fullName, num, num2);
			goto IL_0131;
			IL_0131:
			BitmapSource bitmapSource = (BitmapSource)obj;
			if (num3 == 0)
			{
				ClearInvalidItemsInPool();
				_bitmapPool[new ValueTuple<string, int, int>(fullName, num, num2)] = new WeakReference<BitmapSource>(bitmapSource);
				_naturalSizeCache[fullName] = size;
			}
			if (viewbox != new Rect(0.0, 0.0, 1.0, 1.0))
			{
				viewbox = viewbox.ToAbsoluteViewbox(size);
				bitmapSource = Crop(bitmapSource, viewbox.ToScaledRect(size2.Width / size.Width, size2.Height / size.Height).ToInt32Rect());
			}
			return new ValueTuple<BitmapSource, Size, Size>(bitmapSource, size, size2);
		}

		internal void Clear()
		{
			_bitmapPool.Clear();
		}

		private Size? GetImageSizeFromCache(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			Size value;
			if (!_naturalSizeCache.TryGetValue(fileName, out value))
			{
				return null;
			}
			return value;
		}

		private void ClearInvalidItemsInPool()
		{
			List<ValueTuple<string, int, int>> list = new List<ValueTuple<string, int, int>>();
			foreach (KeyValuePair<ValueTuple<string, int, int>, WeakReference<BitmapSource>> item in _bitmapPool)
			{
				BitmapSource target;
				if (!item.Value.TryGetTarget(out target))
				{
					list.Add(item.Key);
				}
			}
			lock (_bitmapPool)
			{
				foreach (var item2 in list)
				{
					WeakReference<BitmapSource> value;
					_bitmapPool.TryRemove(item2, out value);
				}
			}
		}

		private BitmapImage CreateImage(string fileName, int width, int height)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.None;
			bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
			bitmapImage.UriSource = new Uri(fileName, UriKind.Absolute);
			bitmapImage.DecodePixelWidth = width;
			bitmapImage.DecodePixelHeight = height;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}

		internal static CroppedBitmap Crop(BitmapSource bitmap, Int32Rect viewbox)
		{
			viewbox.X = Math.Max(viewbox.X, 0);
			viewbox.Y = Math.Max(viewbox.Y, 0);
			viewbox.X = Math.Min(viewbox.X, bitmap.PixelWidth);
			viewbox.Y = Math.Min(viewbox.Y, bitmap.PixelHeight);
			viewbox.Width = Math.Max(viewbox.Width, 0);
			viewbox.Height = Math.Max(viewbox.Height, 0);
			viewbox.Width = Math.Min(viewbox.X + viewbox.Width, bitmap.PixelWidth) - viewbox.X;
			viewbox.Height = Math.Min(viewbox.Y + viewbox.Height, bitmap.PixelHeight) - viewbox.Y;
			if (viewbox.Width <= 0 || viewbox.Height <= 0)
			{
				viewbox = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
			}
			CroppedBitmap croppedBitmap = new CroppedBitmap(bitmap, viewbox);
			croppedBitmap.Freeze();
			return croppedBitmap;
		}

		[Conditional("DEBUG")]
		private static void Log(string text)
		{
		}
	}
}
