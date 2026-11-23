using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Threading
{
	internal static class ConcurrentQueueExtension
	{
		
		public static void Clear< T>(this ConcurrentQueue<T> queue)
		{
			T result;
			while (queue.TryDequeue(out result))
			{
			}
		}
	}
}
