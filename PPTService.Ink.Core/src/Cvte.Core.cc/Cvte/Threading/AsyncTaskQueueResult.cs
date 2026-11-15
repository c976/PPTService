using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Threading
{
	
	
	public class AsyncTaskQueueResult< T>
	{
		public bool IsInvalid { get; }

		public T Result { get; }

		internal AsyncTaskQueueResult(bool isInvalid, T result)
		{
			IsInvalid = isInvalid;
			Result = result;
		}
	}
}
