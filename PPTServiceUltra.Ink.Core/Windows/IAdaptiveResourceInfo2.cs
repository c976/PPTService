using System.Collections.Generic;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public interface IAdaptiveResourceInfo2 : IAdaptiveResourceInfo
	{
		Dictionary<Size, string> AdaptiveResourceFormats2 { get; }

		void AssignAdaptiveValue(AdaptiveCondition condition, Size value);
	}
}
