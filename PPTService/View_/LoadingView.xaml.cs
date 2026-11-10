using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Cvte.Windows;

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
			// 进度跑满后直接关闭：不再 ClearData 把圆环归零（原版会先显示 0% 再关闭）。
			// 资源键原代码拼作 Immediatly，与 XAML 的 Immediately 不匹配，淡出动画从未触发。
			(RootGrid.Resources["StoryBoard.HiddenRootGridImmediately"] as Storyboard)?.Begin();
			LoadingScreen<LoadingView>.Current.Hide();
			DispatcherTask.Start(30.0, () =>
			{
				_frame.Continue = false;
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
				_frame.Continue = false;
				LoadingScreen<LoadingView>.Current.Hide();
			});
		}

		private void StoryBoard_HiddenRootGridImmediately_OnCompleted(object sender, EventArgs e)
		{
			_frame.Continue = false;
		}

		private void ClearData()
		{
			_progress = 0;
			LoadProgressBar.Value = 0.0;
			LoadProgressTextBlock.Text = "0%";
		}

	}
}
