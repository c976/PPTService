using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Threading
{
	
	
	public class ContinuousPartOperation
	{
		
		public sealed class Awaiter : INotifyCompletion
		{
			private readonly ContinuousPartOperation _owner;

			public bool IsCompleted => _owner.IsCompleted;

			internal Awaiter(ContinuousPartOperation owner)
			{
				_owner = owner;
			}

			public void OnCompleted(Action continuation)
			{
				if (IsCompleted)
				{
					continuation?.Invoke();
					return;
				}
				ContinuousPartOperation owner = _owner;
				owner._continuation = (Action)Delegate.Combine(owner._continuation, continuation);
			}

			[DebuggerStepThrough]
			public void GetResult()
			{
				_owner._source.Task.GetAwaiter().GetResult();
			}
		}

		
		private readonly TaskCompletionSource<object> _source;

		private readonly Awaiter _awaiter;

		
		private Action _continuation;

		
		private Exception _exception;

		internal bool IsCompleted { get; private set; }

		internal ContinuousPartOperation()
		{
			_source = new TaskCompletionSource<object>();
			_awaiter = new Awaiter(this);
		}

		internal void Complete()
		{
			IsCompleted = true;
			if (_exception == null)
			{
				_source.SetResult(null);
			}
			else
			{
				_source.SetException(_exception);
			}
			Action continuation = _continuation;
			_continuation = null;
			continuation?.Invoke();
		}

		internal void UpdateException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			_exception = exception;
		}

		internal void CleanException()
		{
			_exception = null;
		}

		public Awaiter GetAwaiter()
		{
			return _awaiter;
		}

		public void Wait()
		{
			_source.Task.GetAwaiter().GetResult();
		}
	}
}
