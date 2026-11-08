using System;
using System.Windows.Threading;

namespace PPTService
{
	public class DispatcherTask
	{
		private readonly Action _action;

		private readonly DispatcherTimer _timer;

		private readonly int _loopCount;

		private int _hitCount;

		public bool HasStarted { get; private set; }

		public bool SwallowsException { get; set; }

		public static DispatcherTask Start(Action action)
		{
			return Start(0.0, action);
		}

		public static DispatcherTask Start(double milliseconds, Action action)
		{
			DispatcherTask dispatcherTask = new DispatcherTask(milliseconds, action);
			dispatcherTask.Start();
			return dispatcherTask;
		}

		public static DispatcherTask StartLoop(int loopCount, double intervalMilliseconds, Action action)
		{
			DispatcherTask dispatcherTask = new DispatcherTask(intervalMilliseconds, action, loopCount);
			dispatcherTask.Start();
			return dispatcherTask;
		}

		public static DispatcherTask StartInfiniteLoop(double intervalMilliseconds, Action action)
		{
			DispatcherTask dispatcherTask = new DispatcherTask(intervalMilliseconds, action, -1);
			dispatcherTask.Start();
			return dispatcherTask;
		}

		public void Start()
		{
			if (HasStarted)
			{
				throw new InvalidOperationException("This task has started already.");
			}
			_timer.Start();
			HasStarted = true;
		}

		public void Stop()
		{
			_timer.Stop();
			HasStarted = false;
		}

		public void Restart()
		{
			_hitCount = 0;
			_timer.Stop();
			_timer.Start();
			HasStarted = true;
		}

		public DispatcherTask(Action action)
			: this(0.0, action, 1)
		{
		}

		public DispatcherTask(double milliseconds, Action action)
			: this(milliseconds, action, 1)
		{
		}

		public DispatcherTask(double milliseconds, Action action, int loopCount)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (milliseconds < 0.0)
			{
				throw new ArgumentOutOfRangeException("milliseconds");
			}
			_action = action;
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(milliseconds)
			};
			_timer.Tick += OnTimerTick;
			_loopCount = loopCount;
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			_hitCount++;
			DispatcherTimer dispatcherTimer = (DispatcherTimer)sender;
			if (_hitCount == _loopCount)
			{
				dispatcherTimer.Stop();
			}
			if (SwallowsException)
			{
				try
				{
					_action();
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
			_action();
		}
	}
}
