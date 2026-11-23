namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	internal interface IParagraphSegment : ISegment
	{
		bool IsDeleted { get; }

		int TotalLength { get; }

		int DelimiterLength { get; }

		int ParagraphNumber { get; }
	}
}
