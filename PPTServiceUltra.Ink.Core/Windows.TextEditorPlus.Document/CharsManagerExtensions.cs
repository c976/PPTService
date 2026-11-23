using System;
using System.Collections.Generic;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	internal static class CharsManagerExtensions
	{
		public static bool IsAllWhiteSpace(this CharsManager charsManager, int offset, int length)
		{
			int val = offset + length;
			val = Math.Min(val, charsManager.CharCount);
			List<ICharObject> chars = charsManager.Chars;
			for (int i = offset; i < val; i++)
			{
				if (!string.IsNullOrWhiteSpace(chars[i].ToText()))
				{
					return false;
				}
			}
			return true;
		}
	}
}
