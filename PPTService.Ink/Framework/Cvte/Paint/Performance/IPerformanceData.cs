using System.Collections.Generic;

namespace Cvte.Paint.Performance
{
	public interface IPerformanceData
	{
		string Tag { get; }

		string Action { get; }

		string Detail { get; }

		IReadOnlyDictionary<string, long> Metrics { get; }
	}
}
