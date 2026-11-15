using System.Windows.Threading;

namespace PPTService.Paint.Performance
{
	public interface IContinuousProgressPerformanceCollector
	{
		void CollectUIDelay(DispatcherPriority priority = DispatcherPriority.Background);

		void End();
	}
}
