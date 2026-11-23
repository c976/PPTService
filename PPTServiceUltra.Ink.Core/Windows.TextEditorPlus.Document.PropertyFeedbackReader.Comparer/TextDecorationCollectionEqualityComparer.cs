using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class TextDecorationCollectionEqualityComparer : IEqualityComparer<TextDecorationCollection>
	{
		public bool Equals(TextDecorationCollection x, TextDecorationCollection y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			return x.SequenceEqual(y);
		}

		public int GetHashCode(TextDecorationCollection obj)
		{
			throw new NotSupportedException();
		}
	}
}
