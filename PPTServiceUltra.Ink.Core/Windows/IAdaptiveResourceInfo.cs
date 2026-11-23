using System.Collections.Generic;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public interface IAdaptiveResourceInfo
	{
		Dictionary<double, string> AdaptiveResourceFormats { get; }

		void AssignAdaptiveValue(AdaptiveCondition condition, double value);
	}
}
