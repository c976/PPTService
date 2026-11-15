using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Threading
{
	
	
	public class AsyncAutoResetEvent : IDisposable
	{
		private static readonly Task<bool> CompletedSourceTask = Task.FromResult(true);

		private bool _isDisposed;

		private readonly object _locker = new object();

		private readonly Queue<TaskCompletionSource<bool>> _waitQueue = new Queue<TaskCompletionSource<bool>>();

		private bool _isSignaled;

		public AsyncAutoResetEvent(bool initialState)
		{
			_isSignaled = initialState;
		}

		~AsyncAutoResetEvent()
		{
			Dispose();
		}

		public Task<bool> WaitOneAsync()
		{
			lock (_locker)
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException("AsyncAutoResetEvent");
				}
				if (_isSignaled)
				{
					_isSignaled = false;
					return CompletedSourceTask;
				}
				TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
				_waitQueue.Enqueue(taskCompletionSource);
				return taskCompletionSource.Task;
			}
		}

		public void Set()
		{
			TaskCompletionSource<bool> taskCompletionSource = null;
			bool result;
			lock (_locker)
			{
				if (_waitQueue.Count > 0)
				{
					taskCompletionSource = _waitQueue.Dequeue();
				}
				if (taskCompletionSource == null && !_isSignaled)
				{
					_isSignaled = true;
				}
				result = !_isDisposed;
			}
			taskCompletionSource?.SetResult(result);
		}

		public void Dispose()
		{
			lock (_locker)
			{
				_isDisposed = true;
			}
			GC.SuppressFinalize(this);
			while (true)
			{
				lock (_locker)
				{
					if (_waitQueue.Count == 0)
					{
						break;
					}
				}
				Set();
			}
		}
	}
}
