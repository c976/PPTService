using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using PPTService.Core.Windows;

namespace PPTService
{
	public partial class LoadingView : UserControl, IComponentConnector
	{
		private int _progress;

		private DispatcherFrame _frame;

		private readonly DispatcherTimer _timer;

		public Action CancelAction { get; set; }

		public string Tip
		{
			set
			{
				TipTextBlock.Text = value;
			}
		}

		public int Progress { get; set; }

		public LoadingView()
		{
			InitializeComponent();
			// 底部原本是版权信息，改为显示构建版本号（含 git 短 hash）
			VersionTextBlock.Text = GitVersionInfo.InformationalVersion;
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(3.0)
			};
			_timer.Tick += _timer_Tick;
			Loaded += LoadingView_Loaded;
		}

		private void LoadingView_Loaded(object sender, RoutedEventArgs e)
		{
			Opacity = 1.0;
			ClearData();
			CancelButton.IsEnabled = true;
			(RootGrid.Resources["StoryBoard.ShowRootGrid"] as Storyboard)?.Begin();
			Window window = Window.GetWindow(this);
			if (window != null)
			{
				WindowStateHelper.LoadingWindowIntPtr = new WindowInteropHelper(window).Handle;
			}
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			if (_progress < Progress)
			{
				_progress++;
				LoadProgressBar.Value = _progress;
				LoadProgressTextBlock.Text = _progress + "%";
			}
			if (_progress >= 100)
			{
				_timer.Stop();
				// 跑满后直接隐藏，不再 ClearData 把圆环归零（原版会闪一下 0%）
				(RootGrid.Resources["StoryBoard.HiddenRootGridImmediately"] as Storyboard)?.Begin();
				LoadingScreen<LoadingView>.Current.Hide();
				// 同步结束消息循环，不依赖动画 Completed / 定时器，避免概率性卡在 99%
				if (_frame != null)
				{
					_frame.Continue = false;
				}
				DispatcherTask.Start(30.0, () =>
				{
					if (_frame != null)
					{
						_frame.Continue = false;
					}
				});
			}
		}

		public void Start()
		{
			_timer.Start();
			_frame = new DispatcherFrame();
		}

		public void PushFrame()
		{
			Dispatcher.PushFrame(_frame);
		}

		public void Continue()
		{
			(RootGrid.Resources["StoryBoard.HiddenRootGridImmediately"] as Storyboard)?.Begin();
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			CancelAction?.Invoke();
			Tip = "正在取消...";
			CancelButton.IsEnabled = false;
		}

		public void Cancel()
		{
			Opacity = 0.0;
			ClearData();
			(RootGrid.Resources["StoryBoard.HiddenRootGridImmediately"] as Storyboard)?.Begin();
			DispatcherTask.Start(500.0, () =>
			{
				if (_frame != null)
				{
					_frame.Continue = false;
				}
				LoadingScreen<LoadingView>.Current.Hide();
			});
		}

		private void StoryBoard_HiddenRootGridImmediately_OnCompleted(object sender, EventArgs e)
		{
			if (_frame != null)
			{
				_frame.Continue = false;
			}
		}

		private void ClearData()
		{
			_progress = 0;
			LoadProgressBar.Value = 0.0;
			LoadProgressTextBlock.Text = "0%";
		}

	}
}
