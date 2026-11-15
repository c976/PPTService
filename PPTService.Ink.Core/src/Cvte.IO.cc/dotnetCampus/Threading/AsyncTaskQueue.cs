using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace dotnetCampus.Threading
{
	
	
	internal class AsyncTaskQueue : IDisposable
	{
		private AwaitableTask _lastDoingTask;

		private bool _isDisposed;

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

		
		
		public async Task<ValueTuple<bool, T>> ExecuteAsync< T>(Func<Task<T>> func)
		{
			AwaitableTask<Task<T>> task = GetExecutableTask(func);
			T item = await (await task);
			if (!task.IsValid)
			{
				item = default(T);
			}
			return new ValueTuple<bool, T>(task.IsValid, item);
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
			while (!_isDisposed)
			{
				if (_queue.Count == 0)
				{
					await _autoResetEvent.WaitOneAsync().ConfigureAwait(false);
				}
				AwaitableTask task;
				while (TryGetNextTask(out task))
				{
					if (task.Executable && !_isDisposed)
					{
						if (UseSingleThread)
						{
							task.RunSynchronously();
						}
						else
						{
							task.Start();
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
			if (!_isDisposed)
			{
				_queue.Clear();
				_autoResetEvent.Dispose();
				_isDisposed = true;
			}
		}
	}
}
