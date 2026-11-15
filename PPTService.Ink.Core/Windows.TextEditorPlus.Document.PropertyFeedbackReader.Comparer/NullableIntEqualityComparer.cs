using System;
using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class NullableIntEqualityComparer : IEqualityComparer<int?>
	{
		public bool Equals(int? x, int? y)
		{
			if (!x.HasValue || !y.HasValue)
			{
				return false;
			}
			return x == y;
		}

		public int GetHashCode(int? obj)
		{
			throw new NotSupportedException();
		}
	}
}
