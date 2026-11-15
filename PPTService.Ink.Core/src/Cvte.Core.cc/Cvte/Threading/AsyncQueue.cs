using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Threading
{
	
	
	public class AsyncQueue< T> : IDisposable, IAsyncDisposable
	{
		
		private class CurrentFinishedTask : IDisposable
		{
			private readonly TaskCompletionSource<bool> _currentFinishedTaskCompletionSource = new TaskCompletionSource<bool>();

			private readonly AsyncQueue<T> _asyncQueue;

			public CurrentFinishedTask(AsyncQueue<T> asyncQueue)
			{
				_asyncQueue = asyncQueue;
				lock (_asyncQueue)
				{
					_asyncQueue.CurrentFinished += CurrentFinished;
				}
			}

			private void CurrentFinished(object sender, EventArgs e)
			{
				_currentFinishedTaskCompletionSource.TrySetResult(true);
			}

			public async Task WaitForCurrentFinished()
			{
				await _currentFinishedTaskCompletionSource.Task;
			}

			public void Dispose()
			{
				lock (_asyncQueue)
				{
					_currentFinishedTaskCompletionSource.TrySetResult(true);
					_asyncQueue.CurrentFinished -= CurrentFinished;
				}
			}
		}

		private readonly SemaphoreSlim _semaphoreSlim;

		private readonly ConcurrentQueue<T> _queue;

		private int _dequeueAsyncEnterCount;

		private bool _isDisposing;

		private bool _isDisposed;

		public int Count => _queue.Count;

		
		
		
		private event EventHandler CurrentFinished;

		public AsyncQueue()
		{
			_semaphoreSlim = new SemaphoreSlim(0);
			_queue = new ConcurrentQueue<T>();
		}

		public void Enqueue(T item)
		{
			ThrowIfDisposing();
			_queue.Enqueue(item);
			_semaphoreSlim.Release();
		}

		public void EnqueueRange(IEnumerable<T> source)
		{
			ThrowIfDisposing();
			int num = 0;
			foreach (T item in source)
			{
				_queue.Enqueue(item);
				num++;
			}
			_semaphoreSlim.Release(num);
		}

		public async Task<T> DequeueAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Interlocked.Increment(ref _dequeueAsyncEnterCount);
			try
			{
				while (!_isDisposed)
				{
					await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
					T result;
					if (_queue.TryDequeue(out result))
					{
						return result;
					}
					lock (_queue)
					{
						CurrentFinished?.Invoke(this, EventArgs.Empty);
					}
				}
				return default(T);
			}
			finally
			{
				Interlocked.Decrement(ref _dequeueAsyncEnterCount);
			}
		}

		public async Task WaitForCurrentFinished()
		{
			if (_queue.Count == 0)
			{
				return;
			}
			using (CurrentFinishedTask currentFinishedTask = new CurrentFinishedTask(this))
			{
				if (_queue.Count != 0)
				{
					await currentFinishedTask.WaitForCurrentFinished();
				}
			}
		}

		public void Dispose()
		{
			ThrowIfDisposing();
			_isDisposing = true;
			_isDisposed = true;
			_queue.Clear();
			if (_dequeueAsyncEnterCount > 0)
			{
				_semaphoreSlim.Release(_dequeueAsyncEnterCount);
			}
			_semaphoreSlim.Dispose();
		}

		public async Task DisposeAsync()
		{
			ThrowIfDisposing();
			_isDisposing = true;
			await WaitForCurrentFinished();
			if (_queue.Count != 0)
			{
				await WaitForCurrentFinished();
			}
			_isDisposed = true;
			_semaphoreSlim.Release(int.MaxValue);
			_semaphoreSlim.Dispose();
		}

		private void ThrowIfDisposing()
		{
			if (_isDisposing)
			{
				throw new ObjectDisposedException("AsyncQueue");
			}
		}
	}
}
