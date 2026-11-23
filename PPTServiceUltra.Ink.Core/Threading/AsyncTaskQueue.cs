using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Threading
{
	
	
	public sealed class AsyncTaskQueue : IDisposable
	{
		private AwaitableTask _lastDoingTask;

		private bool _isDisposed;

		private bool _isDisposing;

		private readonly ConcurrentQueue<AwaitableTask> _queue = new ConcurrentQueue<AwaitableTask>();

		private readonly AsyncAutoResetEvent _autoResetEvent;

		private bool _autoCancelPreviousTask;

		public bool UseSingleThread { get; set; } = true;

		public bool AutoCancelPreviousTask
		{
			get
			{
				return _autoCancelPreviousTask;
			}
			set
			{
				if (_lastDoingTask != null)
				{
					throw new InvalidOperationException("此属性应该是在创建对象完成之后给定，不允许在任务执行过程中更改");
				}
				_autoCancelPreviousTask = value;
			}
		}

		private object Locker => _queue;

		public AsyncTaskQueue()
		{
			_autoResetEvent = new AsyncAutoResetEvent(false);
			InternalRunning();
		}

		public async Task<AsyncTaskQueueResult<T>> ExecuteAsync< T>(Func<Task<T>> func)
		{
			AwaitableTask<Task<T>> task = GetExecutableTask(func);
			T result = await (await task);
			if (!task.IsValid)
			{
				result = default(T);
			}
			return new AsyncTaskQueueResult<T>(task.IsValid, result);
		}

		public async Task<bool> ExecuteAsync< T>(Func<Task> func)
		{
			AwaitableTask<Task> task = GetExecutableTask(func);
			await (await task);
			return task.IsValid;
		}

		private AwaitableTask GetExecutableTask(Action action)
		{
			AwaitableTask awaitableTask = new AwaitableTask(new Task(action));
			AddPendingTaskToQueue(awaitableTask);
			return awaitableTask;
		}

		private AwaitableTask<TResult> GetExecutableTask< TResult>(Func<TResult> function)
		{
			AwaitableTask<TResult> awaitableTask = new AwaitableTask<TResult>(new Task<TResult>(function));
			AddPendingTaskToQueue(awaitableTask);
			return awaitableTask;
		}

		private void AddPendingTaskToQueue(AwaitableTask task)
		{
			lock (Locker)
			{
				_queue.Enqueue(task);
				_autoResetEvent.Set();
			}
		}

		private async void InternalRunning()
		{
			while (!_isDisposing)
			{
				Task<bool> task = null;
				bool flag = _queue.Count == 0;
				lock (Locker)
				{
					if (_isDisposing || _isDisposed)
					{
						break;
					}
					if (flag)
					{
						task = _autoResetEvent.WaitOneAsync();
					}
				}
				if (flag && !(await task.ConfigureAwait(false)))
				{
					break;
				}
				AwaitableTask task2;
				while (TryGetNextTask(out task2))
				{
					if (task2.Executable && !_isDisposing)
					{
						if (UseSingleThread)
						{
							task2.RunSynchronously();
						}
						else
						{
							task2.Start();
						}
					}
				}
			}
		}

		private bool TryGetNextTask(out AwaitableTask task)
		{
			task = null;
			while (_queue.Count > 0)
			{
				if (_queue.TryDequeue(out task) && (!AutoCancelPreviousTask || _queue.Count == 0))
				{
					_lastDoingTask?.MarkTaskInvalid();
					_lastDoingTask = task;
					return true;
				}
				task.SetNotExecutable();
			}
			return false;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~AsyncTaskQueue()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}
			lock (Locker)
			{
				if (!_isDisposed)
				{
					_isDisposing = true;
					_queue.Clear();
					_autoResetEvent.Dispose();
					_isDisposed = true;
				}
			}
		}
	}
}
