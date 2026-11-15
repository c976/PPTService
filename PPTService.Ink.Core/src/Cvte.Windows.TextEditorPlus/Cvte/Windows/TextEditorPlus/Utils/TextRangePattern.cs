using System;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public class TextRangePattern
	{
		public char MinChar { get; }

		public char MaxChar { get; }

		public TextRangePattern(char minChar, char maxChar)
		{
			MinChar = minChar;
			MaxChar = maxChar;
		}

		public bool IsInRange(char c)
		{
			if (c >= MinChar)
			{
				return c <= MaxChar;
			}
			return false;
		}

		public bool ContainInRange(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			foreach (char c in text)
			{
				if (c >= MinChar && c <= MaxChar)
				{
					return true;
				}
			}
			return false;
		}

		public bool AreAllInRange([NotNull] string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			foreach (char c in text)
			{
				if (c < MinChar || c > MaxChar)
				{
					return false;
				}
			}
			return true;
		}
	}
}
