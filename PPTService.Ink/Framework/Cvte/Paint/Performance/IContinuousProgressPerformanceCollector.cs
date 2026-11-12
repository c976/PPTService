using System.Windows.Threading;

namespace Cvte.Paint.Performance
{
	public interface IContinuousProgressPerformanceCollector
	{
		void CollectUIDelay(DispatcherPriority priority = DispatcherPriority.Background);

		void End();
	}
}
