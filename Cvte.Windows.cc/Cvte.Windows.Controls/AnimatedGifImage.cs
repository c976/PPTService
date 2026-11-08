using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Cvte.Windows.Controls
{
	public class AnimatedGifImage : System.Windows.Controls.Image
	{
		public delegate void FrameUpdatedEventHandler();

		private Bitmap _bitmap;

		private BitmapSource _bitmapSource;

		public static readonly DependencyProperty GifFilePathProperty = DependencyProperty.Register("GifFilePath", typeof(string), typeof(AnimatedGifImage), new PropertyMetadata(null, OnGifGifFilePathChanged));

		public string GifFilePath
		{
			get
			{
				return (string)GetValue(GifFilePathProperty);
			}
			set
			{
				SetValue(GifFilePathProperty, value);
			}
		}

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			Unloaded += AnimatedGifImage_Unloaded;
		}

		private void AnimatedGifImage_Loaded()
		{
			if (4 != 0)
			{
				_bitmap = new Bitmap(GifFilePath);
				_bitmapSource = GetBitmapSource();
				Source = _bitmapSource;
			}
		}

		private void AnimatedGifImage_Unloaded(object sender, RoutedEventArgs e)
		{
			StopAnimate();
		}

		public void StartAnimate()
		{
			ImageAnimator.Animate(_bitmap, OnFrameChanged);
		}

		public void StopAnimate()
		{
			ImageAnimator.StopAnimate(_bitmap, OnFrameChanged);
		}

		private void OnFrameChanged(object sender, EventArgs e)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FrameUpdatedEventHandler(FrameUpdatedCallback));
		}

		private void FrameUpdatedCallback()
		{
			while (true)
			{
				if (2 == 0)
				{
					goto IL_003c;
				}
				ImageAnimator.UpdateFrames();
				if (false || _bitmapSource == null)
				{
					goto IL_001a;
				}
				goto IL_004a;
				IL_004a:
				_bitmapSource.Freeze();
				goto IL_001a;
				IL_001a:
				if (false)
				{
					continue;
				}
				_bitmapSource = GetBitmapSource();
				Source = _bitmapSource;
				InvalidateVisual();
				goto IL_003c;
				IL_003c:
				if (false)
				{
					goto IL_004a;
				}
				break;
			}
		}

		private BitmapSource GetBitmapSource()
		{
			do
			{
				if (false)
				{
					continue;
				}
				IntPtr intPtr = IntPtr.Zero;
				try
				{
					intPtr = _bitmap.GetHbitmap();
					_bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				}
				finally
				{
					bool flag;
					if (7 != 0)
					{
						flag = intPtr != IntPtr.Zero;
						goto IL_0067;
					}
					goto IL_0069;
					IL_0073:
					do
					{
						GC.Collect();
					}
					while (2 == 0);
					GC.WaitForPendingFinalizers();
					goto end_IL_0059;
					IL_0067:
					if (flag)
					{
						goto IL_0069;
					}
					goto IL_0073;
					IL_0069:
					flag = DeleteObject(intPtr);
					if (false)
					{
						goto IL_0067;
					}
					goto IL_0073;
					end_IL_0059:;
				}
			}
			while (3 == 0);
			return _bitmapSource;
		}

		private static void OnGifGifFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			while (true)
			{
				AnimatedGifImage animatedGifImage;
				if (0 == 0)
				{
					animatedGifImage = d as AnimatedGifImage;
				}
				while (true)
				{
					if (animatedGifImage == null)
					{
						if (false)
						{
							break;
						}
						if (5 != 0)
						{
							return;
						}
					}
					else
					{
						animatedGifImage.AnimatedGifImage_Loaded();
						animatedGifImage.StartAnimate();
					}
					if (false)
					{
						continue;
					}
					return;
				}
			}
		}
	}
}
