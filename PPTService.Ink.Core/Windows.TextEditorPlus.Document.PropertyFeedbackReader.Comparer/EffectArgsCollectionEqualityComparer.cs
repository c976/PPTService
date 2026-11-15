using System;
using System.Collections.Generic;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer
{
	internal class EffectArgsCollectionEqualityComparer : IEqualityComparer<EffectArgsCollection>
	{
		public bool Equals(EffectArgsCollection x, EffectArgsCollection y)
		{
			return Converter.AreEquals(x, y);
		}

		public int GetHashCode(EffectArgsCollection obj)
		{
			throw new NotSupportedException();
		}
	}
}
