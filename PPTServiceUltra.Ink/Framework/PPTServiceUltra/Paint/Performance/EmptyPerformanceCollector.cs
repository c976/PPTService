using System;
using System.Windows.Threading;

namespace PPTServiceUltra.Paint.Performance
{
	internal class EmptyPerformanceCollector : IPerformanceCollector
	{
		public IContinuousProgressPerformanceCollector Start(string tag, string action = null, Func<string> detailGenerator = null)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			return new EmptyContinuousProgressPerformanceCollector();
		}

		public void CollectUIDelay(string tag, string action = null, Func<string> detailGenerator = null, DispatcherPriority priority = DispatcherPriority.Background)
		{
		}
	}
}
