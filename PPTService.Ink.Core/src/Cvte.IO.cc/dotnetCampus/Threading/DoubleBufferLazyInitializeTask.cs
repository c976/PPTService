using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace dotnetCampus.Threading
{
	
	
	internal class DoubleBufferLazyInitializeTask< T>
	{
		private readonly Func<List<T>, Task> _runTask;

		private readonly DoubleBufferTask<T> _doubleBufferTask;

		private bool _isInitialized;

		
		private TaskCompletionSource<bool> _waitForInitializationTask;

		private object Locker => _doubleBufferTask;

		public DoubleBufferLazyInitializeTask(Func<List<T>, Task> runTask)
		{
			_runTask = runTask;
			_doubleBufferTask = new DoubleBufferTask<T>(DoInner);
		}

		public void OnInitialized()
		{
			if (_isInitialized)
			{
				throw new InvalidOperationException("禁止多次设置初始化完成");
			}
			_isInitialized = true;
			lock (Locker)
			{
				if (_waitForInitializationTask != null)
				{
					_waitForInitializationTask.SetResult(true);
				}
			}
		}

		public void AddTask(T data)
		{
			_doubleBufferTask.AddTask(data);
		}

		public void Finish()
		{
			_doubleBufferTask.Finish();
		}

		public Task WaitAllTaskFinish()
		{
			return _doubleBufferTask.WaitAllTaskFinish();
		}

		private async Task DoInner(List<T> dataList)
		{
			if (!_isInitialized)
			{
				lock (Locker)
				{
					if (!_isInitialized)
					{
						_waitForInitializationTask = new TaskCompletionSource<bool>();
					}
				}
				if (!_isInitialized)
				{
					await _waitForInitializationTask.Task.ConfigureAwait(false);
				}
				_waitForInitializationTask = null;
			}
			await _runTask(dataList).ConfigureAwait(false);
		}
	}
}
