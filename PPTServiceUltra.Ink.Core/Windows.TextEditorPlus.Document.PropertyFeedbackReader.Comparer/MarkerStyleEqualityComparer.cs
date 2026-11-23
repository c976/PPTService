using System;
using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class MarkerStyleEqualityComparer : IEqualityComparer<MarkerStyle>
	{
		public bool Equals(MarkerStyle x, MarkerStyle y)
		{
			return Converter.AreEquals(x, y);
		}

		public int GetHashCode(MarkerStyle obj)
		{
			throw new NotSupportedException();
		}
	}
}
