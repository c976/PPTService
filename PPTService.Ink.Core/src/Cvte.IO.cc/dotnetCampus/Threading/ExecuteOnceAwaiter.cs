using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace dotnetCampus.Threading
{
	
	
	internal class ExecuteOnceAwaiter< TResult>
	{
		private readonly object _locker = new object();

		private readonly Func<Task<TResult>> _asyncAction;

		private Task<TResult> _executionResult;

		public ExecuteOnceAwaiter(Func<Task<TResult>> asyncAction)
		{
			_asyncAction = asyncAction;
		}

		public Task<TResult> ExecuteAsync()
		{
			lock (_locker)
			{
				if (_executionResult != null)
				{
					return _executionResult;
				}
				_executionResult = _asyncAction();
				return _executionResult;
			}
		}

		public void ResetWhileCompleted()
		{
			lock (_locker)
			{
				if (_executionResult?.IsCompleted ?? false)
				{
					_executionResult = null;
				}
			}
		}
	}
}
