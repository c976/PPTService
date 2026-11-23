using System;
using System.Collections.Generic;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class BrushEqualityComparer : IEqualityComparer<Brush>
	{
		public bool Equals(Brush x, Brush y)
		{
			if (x == null || y == null)
			{
				return false;
			}
			return Converter.AreEquals(x, y);
		}

		public int GetHashCode(Brush obj)
		{
			throw new NotSupportedException();
		}
	}
}
