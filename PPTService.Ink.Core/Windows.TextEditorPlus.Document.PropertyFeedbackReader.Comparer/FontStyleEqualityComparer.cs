using System;
using System.Collections.Generic;
using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class FontStyleEqualityComparer : IEqualityComparer<FontStyle>
	{
		public bool Equals(FontStyle x, FontStyle y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(FontStyle obj)
		{
			throw new NotSupportedException();
		}
	}
}
