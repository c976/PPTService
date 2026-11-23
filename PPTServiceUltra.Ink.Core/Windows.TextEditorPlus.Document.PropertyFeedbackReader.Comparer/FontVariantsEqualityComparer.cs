using System;
using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class FontVariantsEqualityComparer : IEqualityComparer<FontVariants>
	{
		public bool Equals(FontVariants x, FontVariants y)
		{
			return Converter.AreEquals(x, y);
		}

		public int GetHashCode(FontVariants obj)
		{
			throw new NotSupportedException();
		}
	}
}
