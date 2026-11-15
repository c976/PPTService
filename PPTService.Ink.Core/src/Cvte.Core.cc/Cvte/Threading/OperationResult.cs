using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Threading
{
	[StructLayout(LayoutKind.Auto)]
	
	
	
	public struct OperationResult
	{
		public bool Success
		{
			get
			{
				if (Exception == null)
				{
					return !IsCanceled;
				}
				return false;
			}
		}

		public Exception Exception { get; }

		public bool IsCanceled { get; }

		
		public OperationResult(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			Exception = exception;
			IsCanceled = false;
		}

		public OperationResult(bool isSuccessOrCanceled)
		{
			Exception = null;
			IsCanceled = !isSuccessOrCanceled;
		}

		public static implicit operator bool(OperationResult result)
		{
			return result.Success;
		}

		public static implicit operator Exception(OperationResult result)
		{
			return result.Exception;
		}

		
		public static implicit operator OperationResult(Exception exception)
		{
			if (exception != null)
			{
				return new OperationResult(exception);
			}
			return new OperationResult(true);
		}

		public static implicit operator OperationResult(bool isSuccessOrCanceled)
		{
			return new OperationResult(isSuccessOrCanceled);
		}

		public static bool operator true(OperationResult result)
		{
			return result.Success;
		}

		public static bool operator false(OperationResult result)
		{
			return !result.Success;
		}
	}
}
