namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public interface ISegment
	{
		int StartOffset { get; }

		int Length { get; }

		int EndOffset { get; }

		int FrontOffset { get; }

		int BehindOffset { get; }
	}
}
