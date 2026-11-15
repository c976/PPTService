using System;
using System.Collections.Generic;
using System.Windows;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class TextAlignmentEqualityComparer : IEqualityComparer<TextAlignment>
	{
		public bool Equals(TextAlignment x, TextAlignment y)
		{
			return Converter.AreEquals(x, y);
		}

		public int GetHashCode(TextAlignment obj)
		{
			throw new NotSupportedException();
		}
	}
}
