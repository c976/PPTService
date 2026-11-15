namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	internal static class StorageNullValueRepairman
	{
		public static void FixRichTextSaveInfo(RichTextSaveInfo info)
		{
		}

		public static ParagraphSaveInfo FixNullValue(this ParagraphSaveInfo paragraph)
		{
			return paragraph;
		}

		public static RunSaveInfo FixNullValue(this RunSaveInfo textRun)
		{
			return textRun;
		}
	}
}
