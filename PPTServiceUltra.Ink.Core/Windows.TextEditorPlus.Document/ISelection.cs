namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public interface ISelection
	{
		CaretOffset StartOffset { get; set; }

		int Length { get; }

		bool IsEmpty { get; }

		CaretOffset EndOffset { get; set; }

		CaretOffset FrontOffset { get; }

		CaretOffset BehindOffset { get; }

		ISegment ToSegment();
	}
}
