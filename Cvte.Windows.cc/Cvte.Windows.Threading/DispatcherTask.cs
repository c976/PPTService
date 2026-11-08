using System;
using System.Windows.Threading;

namespace Cvte.Windows.Threading
{
	public class DispatcherTask
	{
		private readonly Action _action;

		private readonly DispatcherTimer _timer;

		private readonly int _loopCount;

		private int _hitCount;

		public bool HasStarted { get; private set; }

		public bool SwallowsException { get; set; }

		public static DispatcherTask Start(double milliseconds, Action action)
		{
			DispatcherTask dispatcherTask = new DispatcherTask(milliseconds, action);
			dispatcherTask.Start();
			return dispatcherTask;
		}

		public static DispatcherTask Start(Action action)
		{
			return Start(0.0, action);
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
			while (true)
			{
				if (8 == 0 || 5 == 0)
				{
					return;
				}
				if (HasStarted)
				{
					if (0 == 0)
					{
						throw new InvalidOperationException("This task has started already.");
					}
					continue;
				}
				if (2 != 0)
				{
					DispatcherTimer timer = _timer;
					if (3u != 0)
					{
						timer.Start();
					}
				}
				break;
			}
			HasStarted = true;
		}

		public void Stop()
		{
			_timer.Stop();
			HasStarted = false;
		}

		public void Restart()
		{
			do
			{
				if (8 != 0 && 5 != 0)
				{
					_hitCount = 0;
					continue;
				}
				return;
			}
			while (false);
			DispatcherTimer timer = _timer;
			if (3u != 0)
			{
				timer.Stop();
			}
			if (2 != 0)
			{
				_timer.Start();
			}
			if (5u != 0)
			{
				HasStarted = true;
			}
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
			while (true)
			{
				_hitCount++;
				while (true)
				{
					DispatcherTimer dispatcherTimer = (DispatcherTimer)sender;
					while (true)
					{
						if (_hitCount != _loopCount)
						{
							goto IL_0034;
						}
						goto IL_0084;
						IL_0034:
						if (0 == 0)
						{
							if (SwallowsException)
							{
								try
								{
									_action();
								}
								catch (Exception)
								{
								}
							}
							else
							{
								if (false)
								{
									break;
								}
								if (1 == 0)
								{
									goto end_IL_006c;
								}
								_action();
							}
							if (5 != 0)
							{
								if (6 == 0)
								{
									continue;
								}
								return;
							}
						}
						goto IL_0084;
						IL_0084:
						dispatcherTimer.Stop();
						goto IL_0034;
					}
					continue;
					end_IL_006c:
					break;
				}
			}
		}
	}
}
