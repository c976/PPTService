using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Threading
{
	
	
	public class CountLimitOperationToken
	{
		private readonly long _countLimit;

		private long _passed;

		public ContinuousPartOperation Operation { get; }

		public CountLimitOperationToken(long countLimit)
		{
			Operation = new ContinuousPartOperation();
			_countLimit = ((countLimit < 0) ? long.MaxValue : countLimit);
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

		public void Pass(long countPassed)
		{
			_passed += countPassed;
			if (_passed >= _countLimit && !Operation.IsCompleted)
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
