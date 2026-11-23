using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Windows.Threading
{
	public class DispatcherAsyncOperation : DispatcherObject, IAwaitable<DispatcherAsyncOperation>, IAwaiter, INotifyCompletion
	{
		private Action _continuation;

		private Exception _exception;

		private DispatcherFrame _waitingFrame;

		private DispatcherPriority _priority = DispatcherPriority.Normal;

		public bool IsCompleted { get; protected set; }

		protected DispatcherAsyncOperation()
		{
		}

		public DispatcherAsyncOperation GetAwaiter()
		{
			return this;
		}

		public void GetResult()
		{
			Wait();
		}

		public void Wait()
		{
			if (!IsCompleted)
			{
				_waitingFrame = new DispatcherFrame();
				Dispatcher.PushFrame(_waitingFrame);
				if (_exception != null)
				{
					ExceptionDispatchInfo.Capture(_exception).Throw();
				}
			}
		}

		public DispatcherAsyncOperation ConfigurePriority(DispatcherPriority priority)
		{
			_priority = priority;
			return this;
		}

		public void OnCompleted(Action continuation)
		{
			if (IsCompleted)
			{
				continuation?.Invoke();
			}
			else
			{
				_continuation = (Action)Delegate.Combine(_continuation, continuation);
			}
		}

		internal void ReportResult(Exception exception)
		{
			if (IsCompleted)
			{
				throw new InvalidOperationException("当任务完成后无法通过调用 ReportResult 再次完成任务。请避免重复调用。");
			}
			_exception = exception;
			IsCompleted = true;
			Action continuation = _continuation;
			_continuation = null;
			if (_waitingFrame != null)
			{
				_waitingFrame.Continue = false;
			}
			if (continuation != null)
			{
				Dispatcher.Continue(continuation, _priority).ConfigureAwait(false);
			}
		}

		public static DispatcherAsyncOperation Create([NotNull] out Action<Exception> reportResult)
		{
			DispatcherAsyncOperation dispatcherAsyncOperation = new DispatcherAsyncOperation();
			reportResult = dispatcherAsyncOperation.ReportResult;
			return dispatcherAsyncOperation;
		}

		public static DispatcherAsyncOperation FromResult()
		{
			DispatcherAsyncOperation dispatcherAsyncOperation = new DispatcherAsyncOperation();
			dispatcherAsyncOperation.ReportResult(null);
			return dispatcherAsyncOperation;
		}

		public static DispatcherAsyncOperation FromResult(Exception exception)
		{
			DispatcherAsyncOperation dispatcherAsyncOperation = new DispatcherAsyncOperation();
			dispatcherAsyncOperation.ReportResult(exception);
			return dispatcherAsyncOperation;
		}

		public static DispatcherAsyncOperation FromResult<TResult>(TResult result)
		{
			DispatcherAsyncOperation<TResult> dispatcherAsyncOperation = new DispatcherAsyncOperation<TResult>();
			dispatcherAsyncOperation.ReportResult(result, null);
			return dispatcherAsyncOperation;
		}

		public static DispatcherAsyncOperation FromResult<TResult>(Exception exception)
		{
			DispatcherAsyncOperation<TResult> dispatcherAsyncOperation = new DispatcherAsyncOperation<TResult>();
			dispatcherAsyncOperation.ReportResult(exception);
			return dispatcherAsyncOperation;
		}
	}
	public sealed class DispatcherAsyncOperation<T> : DispatcherAsyncOperation, IAwaitable<DispatcherAsyncOperation<T>, T>, IAwaiter<T>, INotifyCompletion
	{
		private T _result;

		public T Result
		{
			get
			{
				return GetResult();
			}
			private set
			{
				_result = value;
			}
		}

		internal DispatcherAsyncOperation()
		{
		}

		public new DispatcherAsyncOperation<T> GetAwaiter()
		{
			return this;
		}

		public new T GetResult()
		{
			Wait();
			return _result;
		}

		public new DispatcherAsyncOperation<T> ConfigurePriority(DispatcherPriority priority)
		{
			base.ConfigurePriority(priority);
			return this;
		}

		internal void ReportResult(T result, Exception exception)
		{
			Result = result;
			ReportResult(exception);
		}

		public static DispatcherAsyncOperation<T> Create([NotNull] out Action<T, Exception> reportResult)
		{
			DispatcherAsyncOperation<T> dispatcherAsyncOperation = new DispatcherAsyncOperation<T>();
			reportResult = dispatcherAsyncOperation.ReportResult;
			return dispatcherAsyncOperation;
		}

		public static DispatcherAsyncOperation<T> FromResult(T result)
		{
			DispatcherAsyncOperation<T> dispatcherAsyncOperation = new DispatcherAsyncOperation<T>();
			dispatcherAsyncOperation.ReportResult(result, null);
			return dispatcherAsyncOperation;
		}

		public new static DispatcherAsyncOperation<T> FromResult(Exception exception)
		{
			DispatcherAsyncOperation<T> dispatcherAsyncOperation = new DispatcherAsyncOperation<T>();
			dispatcherAsyncOperation.ReportResult(exception);
			return dispatcherAsyncOperation;
		}
	}
}
