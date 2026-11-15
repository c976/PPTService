using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Threading
{
	
	
	public class AsyncManualResetEvent
	{
		private readonly object _locker = new object();

		private TaskCompletionSource<bool> _source;

		public AsyncManualResetEvent(bool initialState)
		{
			_source = new TaskCompletionSource<bool>();
			if (initialState)
			{
				_source.SetResult(true);
			}
		}

		public Task WaitOneAsync()
		{
			lock (_locker)
			{
				return _source.Task;
			}
		}

		public void Set()
		{
			lock (_locker)
			{
				_source.SetResult(true);
			}
		}

		public void Reset()
		{
			lock (_locker)
			{
				if (_source.Task.IsCompleted)
				{
					_source = new TaskCompletionSource<bool>();
				}
			}
		}
	}
}
