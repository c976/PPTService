namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class PhraseSegment
	{
		public int Index { get; set; }

		public int Length { get; set; }

		public string Text { get; set; }

		public bool Contains(int index)
		{
			if (Index <= index)
			{
				return Index + Length > index;
			}
			return false;
		}
	}
}
