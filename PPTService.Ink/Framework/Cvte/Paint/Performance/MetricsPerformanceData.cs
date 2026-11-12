using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cvte.Paint.Performance
{
	public class MetricsPerformanceData : IPerformanceData
	{
		private readonly Func<string> _detailGenerator;

		private string _detail;

		private static readonly Func<string> DefaultDetailGenerator = () => string.Empty;

		public string Tag { get; }

		public string Action { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string Detail => _detail ?? (_detail = _detailGenerator());

		public IReadOnlyDictionary<string, long> Metrics { get; }

		public MetricsPerformanceData(string tag, string action, Func<string> detailGenerator, IReadOnlyDictionary<string, long> metrics)
		{
			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}
			Tag = tag;
			Action = action ?? string.Empty;
			_detailGenerator = detailGenerator ?? DefaultDetailGenerator;
			Metrics = metrics;
		}
	}
}
