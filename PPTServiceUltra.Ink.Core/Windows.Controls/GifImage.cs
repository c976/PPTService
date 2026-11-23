using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	public class GifImage : Image
	{
		private bool _autoPlay = true;

		private string _fileName;

		private bool _isInited;

		private bool _isIniting;

		public string Id { get; }

		public string FileName
		{
			get
			{
				return _fileName;
			}
			set
			{
				if (!_isInited && _isIniting)
				{
					if (!File.Exists(value))
					{
						throw new ArgumentException("不能传入不存在文件 " + value);
					}
					_fileName = value;
					return;
				}
				if (!_isIniting)
				{
					throw new InvalidOperationException("不能在初始化开始之前修改文件");
				}
				if (_isInited)
				{
					throw new InvalidOperationException("不能在初始化之后修改");
				}
			}
		}

		internal static AnimationImageCachePool AnimationImageCachePool { get; } = new AnimationImageCachePool();

		public GifAnimation GifAnimation
		{
			get
			{
				if (!_isInited)
				{
					throw new InvalidOperationException("不能在没有完成初始化之前获取解析方法");
				}
				return AnimationImageCachePool.GetGifAnimation(FileName);
			}
		}

		public GifImage()
		{
			InitializeProperty();
			Id = Guid.NewGuid().ToString("N");
		}

		public override void BeginInit()
		{
			if (_isInited)
			{
				throw new InvalidOperationException("不能多次初始化");
			}
			_isIniting = true;
			base.BeginInit();
		}

		public override void EndInit()
		{
			if (_isInited)
			{
				throw new InvalidOperationException("不能多次初始化");
			}
			if (string.IsNullOrEmpty(FileName))
			{
				throw new ArgumentException("没有在初始化传入文件，请在初始化的时候设置 FileName 为存在的 Gif 文件");
			}
			_isIniting = false;
			_isInited = true;
			Loaded += GifImage_Loaded;
			Unloaded += GifImage_Unloaded;
			GifAnimation gifAnimation = GifAnimation;
			gifAnimation.PlayOnceFinished += GifAnimation_PlayOnceFinished;
			Source = gifAnimation.Source;
			base.EndInit();
		}

		private void GifAnimation_PlayOnceFinished(object sender, EventArgs e)
		{
			if (!((GifAnimation)sender).AutoReplay)
			{
				Dispatcher.InvokeAsync(Stop);
			}
		}

		public void Play()
		{
			if (!_isInited)
			{
				_autoPlay = true;
			}
			else
			{
				AnimationImageCachePool.TryPlay(FileName, this);
			}
		}

		public void Stop()
		{
			if (!_isInited)
			{
				_autoPlay = false;
			}
			else
			{
				AnimationImageCachePool.TryStop(FileName, this);
			}
		}

		private void GifImage_Unloaded(object sender, RoutedEventArgs e)
		{
			AnimationImageCachePool.GetGifAnimation(FileName).SourceChanged -= GifAnimation_SourceChanged;
			Source = null;
		}

		private void GifImage_Loaded(object sender, RoutedEventArgs e)
		{
			GifAnimation gifAnimation = AnimationImageCachePool.GetGifAnimation(FileName);
			gifAnimation.SourceChanged -= GifAnimation_SourceChanged;
			gifAnimation.SourceChanged += GifAnimation_SourceChanged;
			Source = gifAnimation.Source;
			if (_autoPlay)
			{
				gifAnimation.Play();
			}
		}

		private async void GifAnimation_SourceChanged(object sender, ImageSource e)
		{
			await Dispatcher.InvokeAsync(() =>
			{
				Source = e;
			});
		}

		private void InitializeProperty()
		{
			Stretch = Stretch.Fill;
			UseLayoutRounding = true;
			SnapsToDevicePixels = true;
		}
	}
}
