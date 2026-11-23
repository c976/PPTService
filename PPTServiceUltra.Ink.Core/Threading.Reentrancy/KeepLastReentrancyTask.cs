using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Threading.Reentrancy
{
	
	
	public sealed class KeepLastReentrancyTask< TParameter,  TReturn> : ReentrancyTask<TParameter, TReturn>
	{
		
		private class TaskWrapper
		{
			private readonly TaskCompletionSource<TReturn> _taskSource;

			private readonly Func<Task<TReturn>> _task;

			public TReturn Result { get; set; }

			public Exception Exception { get; set; }

			internal TaskWrapper(Func<Task<TReturn>> workingTask)
			{
				_taskSource = new TaskCompletionSource<TReturn>();
				_task = workingTask;
			}

			internal async Task RunAsync()
			{
				try
				{
					Task<TReturn> task = _task();
					if (task == null)
					{
						throw new InvalidOperationException("在指定 KeepLastReentrancyTask 的任务时，方法内不允许返回 null。请至少返回 Task.FromResult<object>(null)。");
					}
					TReturn result = await task.ConfigureAwait(false);
					_taskSource.SetResult(result);
					Result = result;
				}
				catch (Exception exception)
				{
					_taskSource.SetException(exception);
					Exception = exception;
				}
			}

			public void SetResult(TReturn result)
			{
				if (!_taskSource.Task.IsCompleted && !_taskSource.Task.IsFaulted)
				{
					_taskSource.SetResult(result);
				}
			}

			public void SetException(Exception exception)
			{
				if (!_taskSource.Task.IsCompleted && !_taskSource.Task.IsFaulted)
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

		
		private readonly Queue<TaskWrapper> _skipQueue = new Queue<TaskWrapper>();

		public KeepLastReentrancyTask(Func<TParameter, Task<TReturn>> task)
			: base(task)
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
				TaskWrapper runTask = null;
				TaskWrapper result;
				while (_queue.TryDequeue(out result))
				{
					if (runTask != null)
					{
						_skipQueue.Enqueue(runTask);
					}
					runTask = result;
				}
				if (runTask != null)
				{
					await runTask.RunAsync().ConfigureAwait(false);
					if (runTask.Exception != null)
					{
						SetException(runTask.Exception);
					}
					else
					{
						SetResult(runTask.Result);
					}
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

		private void SetException(Exception exception)
		{
			while (_skipQueue.Count > 0)
			{
				_skipQueue.Dequeue().SetException(exception);
			}
		}

		private void SetResult(TReturn result)
		{
			while (_skipQueue.Count > 0)
			{
				_skipQueue.Dequeue().SetResult(result);
			}
		}
	}
}
