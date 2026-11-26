using System;
using System.Windows.Threading;

namespace PPTServiceUltra
{
	public class MouseHelper
	{
		private static bool _isStart;

		private int _preMouseState;

		// 右键状态单独记录：PowerPoint 放映时的原生右键菜单会把附着工具栏压到
		// 放映窗口后面，需要全局监听右键做兜底恢复（左键轮询不受影响）。
		private int _preRightMouseState;

		private readonly DispatcherTimer _timer;

		private static readonly int TimerSpan = 6;

		private static readonly double MouseStateBoundary = 25.0;

		public static MouseHelper Instance { get; set; }

		public event EventHandler<MouseHelperEventArgs> Down;

		public event EventHandler<MouseHelperEventArgs> Move;

		public event EventHandler<MouseHelperEventArgs> Up;

		public event EventHandler<MouseHelperEventArgs> RightDown;

		public event EventHandler<MouseHelperEventArgs> RightUp;

		public MouseHelper()
		{
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(TimerSpan)
			};
			_timer.Tick += _timerTick;
		}

		public void Start()
		{
			if (!_isStart)
			{
				_isStart = true;
				_timer.Start();
			}
		}

		public void Stop()
		{
			if (_isStart)
			{
				_isStart = false;
				_timer.Stop();
			}
		}

		private void _timerTick(object sender, EventArgs e)
		{
			MousePoint lpPoint;
			WindowsApi.GetCursorPos(out lpPoint);
			int num = Math.Abs(WindowsApi.GetKeyState(1));
			if ((double)num > MouseStateBoundary && (double)_preMouseState < MouseStateBoundary)
			{
				OnDown(new MouseHelperEventArgs(lpPoint.X, lpPoint.Y));
				_preMouseState = num;
			}
			if ((double)num > MouseStateBoundary && (double)_preMouseState > MouseStateBoundary)
			{
				OnMove(new MouseHelperEventArgs(lpPoint.X, lpPoint.Y));
				_preMouseState = num;
			}
			if ((double)num < MouseStateBoundary && (double)_preMouseState > MouseStateBoundary)
			{
				OnUp(new MouseHelperEventArgs(lpPoint.X, lpPoint.Y));
				_preMouseState = num;
			}

			int right = Math.Abs(WindowsApi.GetKeyState(2));
			if ((double)right > MouseStateBoundary && (double)_preRightMouseState < MouseStateBoundary)
			{
				OnRightDown(new MouseHelperEventArgs(lpPoint.X, lpPoint.Y));
				_preRightMouseState = right;
			}
			if ((double)right < MouseStateBoundary && (double)_preRightMouseState > MouseStateBoundary)
			{
				OnRightUp(new MouseHelperEventArgs(lpPoint.X, lpPoint.Y));
				_preRightMouseState = right;
			}
		}

		protected virtual void OnDown(MouseHelperEventArgs e)
		{
			Down?.Invoke(this, e);
		}

		protected virtual void OnMove(MouseHelperEventArgs e)
		{
			Move?.Invoke(this, e);
		}

		protected virtual void OnUp(MouseHelperEventArgs e)
		{
			Up?.Invoke(this, e);
		}

		protected virtual void OnRightDown(MouseHelperEventArgs e)
		{
			RightDown?.Invoke(this, e);
		}

		protected virtual void OnRightUp(MouseHelperEventArgs e)
		{
			RightUp?.Invoke(this, e);
		}
	}
}
