using System;
using System.Collections.Generic;
using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class FlowDirectionEqualityComparer : IEqualityComparer<FlowDirection>
	{
		public bool Equals(FlowDirection x, FlowDirection y)
		{
			return x == y;
		}

		public int GetHashCode(FlowDirection obj)
		{
			throw new NotSupportedException();
		}
	}
}
