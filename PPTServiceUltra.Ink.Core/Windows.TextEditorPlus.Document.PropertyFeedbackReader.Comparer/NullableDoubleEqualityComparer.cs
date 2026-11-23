using System;
using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class NullableDoubleEqualityComparer : IEqualityComparer<double?>
	{
		public bool Equals(double? x, double? y)
		{
			if (!x.HasValue || !y.HasValue)
			{
				return false;
			}
			return Nearly.Equals(x.Value, y.Value);
		}

		public int GetHashCode(double? obj)
		{
			throw new NotSupportedException();
		}
	}
}
