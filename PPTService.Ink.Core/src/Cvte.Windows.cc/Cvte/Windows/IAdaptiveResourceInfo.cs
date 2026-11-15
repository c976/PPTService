using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows
{
	public interface IAdaptiveResourceInfo
	{
		Dictionary<double, string> AdaptiveResourceFormats { get; }

		void AssignAdaptiveValue(AdaptiveCondition condition, double value);
	}
}
