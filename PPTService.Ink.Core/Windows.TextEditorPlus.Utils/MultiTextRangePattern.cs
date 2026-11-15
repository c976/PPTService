using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class MultiTextRangePattern
	{
		private char[] TextRange { get; }

		public MultiTextRangePattern(params char[] textRange)
		{
			TextRange = textRange;
		}

		public bool AreAllInRange([NotNull] string text)
		{
			foreach (char c in text)
			{
				bool flag = false;
				for (int j = 0; j < TextRange.Length; j += 2)
				{
					char c2 = TextRange[j];
					char c3 = TextRange[j + 1];
					if (c >= c2 && c <= c3)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}
	}
}
