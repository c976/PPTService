using System;
using System.Collections.Generic;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class FontFamilyEqualityComparer : IEqualityComparer<FontFamily>
	{
		public bool Equals(FontFamily x, FontFamily y)
		{
			return Converter.AreEquals(x, y);
		}

		public int GetHashCode(FontFamily obj)
		{
			throw new NotSupportedException();
		}
	}
}
