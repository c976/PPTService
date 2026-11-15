using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Diagnostics.Trace;

namespace PPTService.Ink.Core.Windows.Threading
{
	public class DispatcherTask
	{
		private readonly Action _action;

		private readonly DispatcherTimer _timer;

		private readonly int _loopCount;

		private int _hitCount;

		public bool HasStarted { get; private set; }

		public bool SwallowsException { get; set; }

		public ITracer Tracer { get; set; } = TraceListener.Instance;

		[Obsolete("请使用 await Task.Delay(milliseconds); action(); 代替 或使用 DispatcherTask.Run 代替")]
		public static DispatcherTask Start(double milliseconds, Action action)
		{
			DispatcherTask dispatcherTask = new DispatcherTask(milliseconds, action);
			dispatcherTask.Start();
			return dispatcherTask;
		}

		public static Task Run(TimeSpan delayTime, [NotNull] Action action, bool continueOnCapturedContext = true)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			return Run((int)delayTime.TotalMilliseconds, action, continueOnCapturedContext);
		}

		public static Task Run(int delayTimeMilliseconds, [NotNull] Action action, bool continueOnCapturedContext = true)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (continueOnCapturedContext)
			{
				return Task.Delay(delayTimeMilliseconds).ContinueWith((Task _) =>
				{
					action();
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			return Task.Delay(delayTimeMilliseconds).ContinueWith((Task _) =>
			{
				action();
			});
		}

		[Obsolete("请使用对应 Dispatcher.Invoke 或 Dispatcher.InvokeAsync 代替")]
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

		[Obsolete("请使用 Dispatcher.InvokeAsync 代替，这个方法有性能问题")]
		public static async Task InvokeAsync<T>(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			await InvokeAsync(() =>
			{
				action();
				return true;
			}, priority);
		}

		[Obsolete("请使用 Dispatcher.InvokeAsync 代替，这个方法有性能问题")]
		public static async Task<T> InvokeAsync<T>(Func<T> func, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			T result = default(T);
			ManualResetEvent resetEvent = new ManualResetEvent(false);
			try
			{
				Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
				{
					result = func();
					resetEvent.Set();
				}), priority);
				await Task.Run(() =>
				{
					resetEvent.WaitOne();
					return result;
				});
			}
			finally
			{
				if (resetEvent != null)
				{
					((IDisposable)resetEvent).Dispose();
				}
			}
			return result;
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
			if (milliseconds < 0.0)
			{
				throw new ArgumentOutOfRangeException("milliseconds");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
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
				catch (Exception exception)
				{
					Tracer?.Trace(exception, null);
					return;
				}
			}
			_action();
		}
	}
}
