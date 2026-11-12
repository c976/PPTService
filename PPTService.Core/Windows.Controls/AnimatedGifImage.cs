using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PPTService.Core.Windows.Controls
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
			_bitmap = new Bitmap(GifFilePath);
			_bitmapSource = GetBitmapSource();
			Source = _bitmapSource;
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
			ImageAnimator.UpdateFrames();
			if (_bitmapSource != null)
			{
				_bitmapSource.Freeze();
			}
			_bitmapSource = GetBitmapSource();
			Source = _bitmapSource;
			InvalidateVisual();
		}

		private BitmapSource GetBitmapSource()
		{
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = _bitmap.GetHbitmap();
				_bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					DeleteObject(intPtr);
				}
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			return _bitmapSource;
		}

		private static void OnGifGifFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AnimatedGifImage animatedGifImage = d as AnimatedGifImage;
			if (animatedGifImage != null)
			{
				animatedGifImage.AnimatedGifImage_Loaded();
				animatedGifImage.StartAnimate();
			}
		}
	}
}
