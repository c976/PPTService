using System.Windows.Threading;

namespace PPTServiceUltra.Paint.Performance
{
	public interface IContinuousProgressPerformanceCollector
	{
		void CollectUIDelay(DispatcherPriority priority = DispatcherPriority.Background);

		void End();
	}
}
