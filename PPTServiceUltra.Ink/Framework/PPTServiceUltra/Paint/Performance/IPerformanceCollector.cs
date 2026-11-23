using System;
using System.Windows.Threading;

namespace PPTServiceUltra.Paint.Performance
{
	public interface IPerformanceCollector
	{
		IContinuousProgressPerformanceCollector Start(string tag, string action = null, Func<string> detailGenerator = null);

		void CollectUIDelay(string tag, string action = null, Func<string> detailGenerator = null, DispatcherPriority priority = DispatcherPriority.Background);
	}
}
