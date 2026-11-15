using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace PPTService.Ink.Core.Windows
{
	public static class ClipboardHelper
	{
		private const int DefaultRetryTimes = 3;

		public static System.Windows.Forms.IDataObject GetDataObject(int retryTimes = 3)
		{
			return TryDo(() => System.Windows.Forms.Clipboard.GetDataObject(), retryTimes);
		}

		public static void SetDataObject(System.Windows.Forms.IDataObject dataObject, int retryTimes = 3)
		{
			TryDo(() =>
			{
				System.Windows.Forms.Clipboard.SetDataObject(dataObject, true);
			}, retryTimes);
		}

		public static void SetImage(BitmapSource image, int retryTimes = 3)
		{
			Bitmap data = _003CSetImage_003Eg__GetBitmap_007C3_0(image);
			SetDataObject(new System.Windows.Forms.DataObject(System.Windows.Forms.DataFormats.Bitmap, data), retryTimes);
		}

		public static object TryGetData(this System.Windows.Forms.IDataObject dataObject, Type format, int retryTimes = 3)
		{
			return TryDo(() => dataObject.GetData(format), retryTimes);
		}

		public static object TryGetData(this System.Windows.Forms.IDataObject dataObject, string format, int retryTimes = 3)
		{
			return TryDo(() => dataObject.GetData(format), retryTimes);
		}

		public static object TryGetData(this System.Windows.Forms.IDataObject dataObject, string format, bool autoConvert, int retryTimes = 3)
		{
			return TryDo(() => dataObject.GetData(format, autoConvert), retryTimes);
		}

		public static void TrySetData(this System.Windows.Forms.IDataObject dataObject, Type format, object data, int retryTimes = 3)
		{
			TryDo(() =>
			{
				dataObject.SetData(format, data);
			}, retryTimes);
		}

		public static void TrySetData(this System.Windows.Forms.IDataObject dataObject, object data, int retryTimes = 3)
		{
			TryDo(() =>
			{
				dataObject.SetData(data);
			}, retryTimes);
		}

		public static void TrySetData(this System.Windows.Forms.IDataObject dataObject, string format, object data, int retryTimes = 3)
		{
			TryDo(() =>
			{
				dataObject.SetData(format, data);
			}, retryTimes);
		}

		public static void TrySetData(this System.Windows.Forms.IDataObject dataObject, string format, object data, bool autoConvert, int retryTimes = 3)
		{
			TryDo(() =>
			{
				dataObject.SetData(format, autoConvert, data);
			}, retryTimes);
		}

		public static bool IsDataObjectFrom(this System.Windows.Forms.IDataObject dataObject, string keyWord)
		{
			string text = dataObject.TryGetData(System.Windows.Forms.DataFormats.Html) as string;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			MatchCollection matchCollection = new Regex("(<meta(.|[\\s\\S])*?>)|(<html(.|[\\s\\S])*?>)", RegexOptions.IgnoreCase).Matches(text);
			if (matchCollection.Count > 0)
			{
				return matchCollection.Cast<object>().Any((object match) => match.ToString().ToLower(CultureInfo.InvariantCulture).Contains(keyWord));
			}
			return false;
		}

		private static void TryDo(Action action, int retryTimes = 3)
		{
			int num = ((retryTimes < 0) ? 1 : (retryTimes + 1));
			for (int i = 0; i < num; i++)
			{
				try
				{
					action();
					break;
				}
				catch (ExternalException)
				{
					Thread.Sleep(10);
				}
			}
		}

		private static T TryDo<T>(Func<T> func, int retryTimes = 3)
		{
			int num = ((retryTimes < 0) ? 1 : (retryTimes + 1));
			for (int i = 0; i < num; i++)
			{
				try
				{
					return func();
				}
				catch (ExternalException)
				{
					Thread.Sleep(10);
				}
			}
			return default(T);
		}

		[CompilerGenerated]
		internal static Bitmap _003CSetImage_003Eg__GetBitmap_007C3_0(BitmapSource source)
		{
			Bitmap bitmap = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
			source.CopyPixels(Int32Rect.Empty, bitmapData.Scan0, bitmapData.Height * bitmapData.Stride, bitmapData.Stride);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
	}
}
