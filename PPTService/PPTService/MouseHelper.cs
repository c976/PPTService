using System;
using System.Windows.Threading;

namespace PPTService
{
	public class MouseHelper
	{
		private static bool _isStart;

		private int _preMouseState;

		private readonly DispatcherTimer _timer;

		private static readonly int TimerSpan = 6;

		private static readonly double MouseStateBoundary = 25.0;

		public static MouseHelper Instance { get; set; }

		public event EventHandler<MouseHelperEventArgs> Down;

		public event EventHandler<MouseHelperEventArgs> Move;

		public event EventHandler<MouseHelperEventArgs> Up;

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
	}
}
