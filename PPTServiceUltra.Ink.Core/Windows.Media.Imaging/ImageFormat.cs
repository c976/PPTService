using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows.Media.Imaging
{
	[DebuggerDisplay("ImageFormat Name={Name}")]
	public sealed class ImageFormat : DispatcherObject
	{
		private class ImageFormatProvider
		{
			private readonly string _name;

			public static Dictionary<string, Func<ImageFormat>> ImageFormatCreatorList { get; }

			private WeakReference<ImageFormat> ImageFormat { get; }

			private static List<ImageFormatProvider> CachedImageFormatProviderList { get; }

			static ImageFormatProvider()
			{
				ImageFormatCreatorList = new Dictionary<string, Func<ImageFormat>>();
				CachedImageFormatProviderList = new List<ImageFormatProvider>();
				_003C_002Ecctor_003Eg__AddImageFormat_007C0_0("PNG");
				_003C_002Ecctor_003Eg__AddImageFormat_007C0_0("JPG");
				_003C_002Ecctor_003Eg__AddImageFormat_007C0_0("BMP");
				_003C_002Ecctor_003Eg__AddImageFormat_007C0_0("GIF");
				_003C_002Ecctor_003Eg__AddImageFormat_007C0_0("TIFF");
				_003C_002Ecctor_003Eg__AddImageFormat_007C0_0("WMP");
			}

			public static ImageFormat GetImageFormat(string name)
			{
				Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
				if (!ImageFormatCreatorList.ContainsKey(name))
				{
					throw new NotSupportedException("暂时不支持" + name + "格式，现在能支持的格式有" + string.Join(" ", ImageFormatCreatorList.Select((KeyValuePair<string, Func<ImageFormat>> x) => x.Key)));
				}
				if (name == "JPEG")
				{
					name = "JPG";
				}
				for (int num = 0; num < CachedImageFormatProviderList.Count; num++)
				{
					ImageFormat target;
					if (!CachedImageFormatProviderList[num].ImageFormat.TryGetTarget(out target))
					{
						CachedImageFormatProviderList.RemoveAt(num);
						num--;
					}
					else if (target.Dispatcher == currentDispatcher && target.Name == name)
					{
						return target;
					}
				}
				ImageFormatProvider imageFormatProvider = new ImageFormatProvider(name);
				CachedImageFormatProviderList.Add(imageFormatProvider);
				ImageFormat target2;
				imageFormatProvider.ImageFormat.TryGetTarget(out target2);
				return target2;
			}

			private ImageFormatProvider(string name)
			{
				if (name == "JPEG")
				{
					name = "JPG";
				}
				ImageFormat = new WeakReference<ImageFormat>(ImageFormatCreatorList[name]());
				_name = name;
			}

			public override string ToString()
			{
				return "ImageFormat.ImageFormatProvider Name=" + _name;
			}

			[CompilerGenerated]
			internal static void _003C_002Ecctor_003Eg__AddImageFormat_007C0_0(string name)
			{
				ImageFormatCreatorList.Add(name, () => new ImageFormat(name));
			}
		}

		private const string PngString = "PNG";

		private const string JpgString = "JPG";

		private const string JpegString = "JPEG";

		private const string BmpString = "BMP";

		private const string GifString = "GIF";

		private const string TiffString = "TIFF";

		private const string WmpString = "WMP";

		private static Dictionary<string, Func<BitmapEncoder>> BitmapEncoderCreatorList { get; }

		public static ImageFormat Png => ImageFormatProvider.GetImageFormat("PNG");

		public static ImageFormat Jpg => ImageFormatProvider.GetImageFormat("JPG");

		public static ImageFormat Bmp => ImageFormatProvider.GetImageFormat("BMP");

		public static ImageFormat Gif => ImageFormatProvider.GetImageFormat("GIF");

		public static ImageFormat Tiff => ImageFormatProvider.GetImageFormat("TIFF");

		public static ImageFormat Wmp => ImageFormatProvider.GetImageFormat("WMP");

		public string Name { get; }

		static ImageFormat()
		{
			Func<BitmapEncoder> value = () => new JpegBitmapEncoder();
			BitmapEncoderCreatorList = new Dictionary<string, Func<BitmapEncoder>>
			{
				{
					"PNG",
					() => new PngBitmapEncoder()
				},
				{ "JPG", value },
				{ "JPEG", value },
				{
					"BMP",
					() => new BmpBitmapEncoder()
				},
				{
					"GIF",
					() => new GifBitmapEncoder()
				},
				{
					"TIFF",
					() => new TiffBitmapEncoder()
				},
				{
					"WMP",
					() => new WmpBitmapEncoder()
				}
			};
		}

		public ImageFormat([NotNull] string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("不能使用空字符串作为文件格式。", "name");
			}
			if (name.StartsWith("."))
			{
				throw new ArgumentException("输入是 name 是图片编码格式，不能使用“.”形式的扩展名。");
			}
			name = name.ToUpperInvariant();
			if (!BitmapEncoderCreatorList.ContainsKey(name))
			{
				throw new NotSupportedException("暂时不支持" + name + "格式，现在能支持的格式有" + string.Join(" ", BitmapEncoderCreatorList.Select((KeyValuePair<string, Func<BitmapEncoder>> x) => x.Key)));
			}
			Name = name;
		}

		public BitmapEncoder CreateEncoder()
		{
			Dispatcher.VerifyAccess();
			return BitmapEncoderCreatorList[Name]();
		}

		public static ImageFormat FromFileName(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (!string.IsNullOrEmpty(extension))
			{
				string text = extension.Substring(1).ToUpperInvariant();
				if (text == "JPEG")
				{
					text = "JPG";
				}
				if (ImageFormatProvider.ImageFormatCreatorList.ContainsKey(text))
				{
					return ImageFormatProvider.GetImageFormat(text);
				}
			}
			return null;
		}

		private static bool EqualsInner(ImageFormat left, ImageFormat right)
		{
			if (left == null && right == null)
			{
				return true;
			}
			if (left != null && right != null)
			{
				return left.Name == right.Name;
			}
			return false;
		}

		private static bool EqualsInner(ImageFormat left, object right)
		{
			if (left == null && right == null)
			{
				return true;
			}
			ImageFormat imageFormat = right as ImageFormat;
			if (imageFormat != null)
			{
				return EqualsInner(left, imageFormat);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return EqualsInner(this, obj);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}
