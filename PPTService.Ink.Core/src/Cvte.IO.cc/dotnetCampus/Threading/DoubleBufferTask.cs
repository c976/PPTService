using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace dotnetCampus.Threading
{
	
	
	internal class DoubleBufferTask<T,  TU> : IAsyncDisposable where T : class, ICollection<TU>
	{
		private bool _isDoing;

		private readonly Func<T, Task> _doTask;

		private TaskCompletionSource<bool> FinishTask { get; } = new TaskCompletionSource<bool>();

		private DoubleBuffer<T, TU> DoubleBuffer { get; }

		
		
		
		private event EventHandler Finished;

		public DoubleBufferTask(T aList, T bList, Func<T, Task> doTask)
		{
			_doTask = doTask;
			DoubleBuffer = new DoubleBuffer<T, TU>(aList, bList);
		}

		public void AddTask(TU t)
		{
			DoubleBuffer.Add(t);
			DoInner();
		}

		private async void DoInner()
		{
			if (_isDoing)
			{
				return;
			}
			lock (DoubleBuffer)
			{
				if (_isDoing)
				{
					return;
				}
				_isDoing = true;
			}
			await DoubleBuffer.DoAllAsync(_doTask).ConfigureAwait(false);
			lock (DoubleBuffer)
			{
				_isDoing = false;
				Finished?.Invoke(this, EventArgs.Empty);
			}
		}

		public void Finish()
		{
			lock (DoubleBuffer)
			{
				if (!_isDoing)
				{
					FinishTask.SetResult(true);
					return;
				}
				Finished +=  (object sender, EventArgs args) =>
				{
					FinishTask.SetResult(true);
				};
			}
		}

		public Task WaitAllTaskFinish()
		{
			return FinishTask.Task;
		}

		public async Task DisposeAsync()
		{
			Finish();
			await WaitAllTaskFinish().ConfigureAwait(false);
		}
	}
	
	
	internal class DoubleBufferTask< T> : DoubleBufferTask<List<T>, T>
	{
		public DoubleBufferTask(Func<List<T>, Task> doTask)
			: base(new List<T>(), new List<T>(), doTask)
		{
		}
	}
}
