using System.Windows.Threading;

namespace PPTServiceUltra.Paint.Performance
{
	internal class EmptyContinuousProgressPerformanceCollector : IContinuousProgressPerformanceCollector
	{
		public void CollectUIDelay(DispatcherPriority priority = DispatcherPriority.Background)
		{
		}

		public void End()
		{
		}
	}
}
