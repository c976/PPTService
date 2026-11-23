using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Threading
{
	
	
	public class TimeoutOperationToken
	{
		private readonly TimeSpan _timeout;

		private TimeSpan _passed;

		public ContinuousPartOperation Operation { get; }

		public TimeoutOperationToken(TimeSpan timeout)
		{
			Operation = new ContinuousPartOperation();
			_timeout = ((timeout < TimeSpan.Zero) ? TimeSpan.MaxValue : timeout);
		}

		public void Complete(bool removeIntermediateExceptions = true)
		{
			if (removeIntermediateExceptions)
			{
				Operation.CleanException();
			}
			if (!Operation.IsCompleted)
			{
				Operation.Complete();
			}
		}

		public void Pass(TimeSpan passedTimeSinceLastPass)
		{
			_passed += passedTimeSinceLastPass;
			if (_passed >= _timeout && !Operation.IsCompleted)
			{
				Operation.Complete();
			}
		}

		public void UpdateException(Exception exception)
		{
			if (!Operation.IsCompleted)
			{
				Operation.UpdateException(exception);
			}
		}
	}
}
