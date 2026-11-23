namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	internal static class ParagraphTreeExtension
	{
		internal static ParagraphNode GetParagraphNodeByCaretOffset(this ParagraphTree tree, CaretOffset offset)
		{
			return tree.GetParagraphNodeByDocumentOffset(offset.IsAtParagraphStart ? (offset.Offset + 1) : offset.Offset);
		}
	}
}
