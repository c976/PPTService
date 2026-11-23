using System;
using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class DoubleEqualityComparer : IEqualityComparer<double>
	{
		public bool Equals(double x, double y)
		{
			return Nearly.Equals(x, y);
		}

		public int GetHashCode(double obj)
		{
			throw new NotSupportedException();
		}
	}
}
