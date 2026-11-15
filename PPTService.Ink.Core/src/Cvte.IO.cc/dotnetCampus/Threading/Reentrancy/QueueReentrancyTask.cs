using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace dotnetCampus.Threading.Reentrancy
{
	
	
	internal sealed class QueueReentrancyTask< TParameter,  TReturn> : ReentrancyTask<TParameter, TReturn>
	{
		
		private class TaskWrapper
		{
			private readonly TaskCompletionSource<TReturn> _taskSource;

			private readonly Func<Task<TReturn>> _task;

			internal TaskWrapper(Func<Task<TReturn>> workingTask)
			{
				_taskSource = new TaskCompletionSource<TReturn>();
				_task = workingTask;
			}

			internal async Task RunAsync()
			{
				try
				{
					TReturn result = await _task().ConfigureAwait(false);
					_taskSource.SetResult(result);
				}
				catch (Exception exception)
				{
					_taskSource.SetException(exception);
				}
			}

			internal Task<TReturn> AsTask()
			{
				return _taskSource.Task;
			}
		}

		private volatile int _isRunning;

		private readonly object _locker = new object();

		
		private readonly ConcurrentQueue<TaskWrapper> _queue = new ConcurrentQueue<TaskWrapper>();

		private readonly bool _configureAwait;

		public QueueReentrancyTask(Func<TParameter, Task<TReturn>> task, bool configureAwait)
			: base(task)
		{
			_configureAwait = configureAwait;
		}

		public QueueReentrancyTask(Func<TParameter, Task<TReturn>> task)
			: this(task, false)
		{
		}

		public override Task<TReturn> InvokeAsync(TParameter arg)
		{
			TaskWrapper taskWrapper = new TaskWrapper(() => RunCore(arg));
			_queue.Enqueue(taskWrapper);
			Run();
			return taskWrapper.AsTask();
		}

		private async void Run()
		{
			if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 1)
			{
				lock (_locker)
				{
					if (_isRunning == 1)
					{
						return;
					}
				}
			}
			bool flag = true;
			while (flag)
			{
				TaskWrapper result;
				while (_queue.TryDequeue(out result))
				{
					await result.RunAsync().ConfigureAwait(_configureAwait);
				}
				lock (_locker)
				{
					TaskWrapper result2;
					flag = _queue.TryPeek(out result2);
					if (!flag)
					{
						_isRunning = 0;
					}
				}
			}
		}
	}
}
