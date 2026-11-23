using System.Collections.Generic;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class ParagraphPropertyInfo
	{
		public List<int> PreIndexes;

		public List<int> AfterIndexes;

		public ParagraphProperty PreCurrentParagraphProperty { get; set; }

		public ParagraphProperty AfterCurrentParagraphProperty { get; set; }
	}
}
