using System;
using System.Collections.Generic;
using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class FontWeightEqualityComparer : IEqualityComparer<FontWeight>
	{
		public bool Equals(FontWeight x, FontWeight y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(FontWeight obj)
		{
			throw new NotSupportedException();
		}
	}
}
