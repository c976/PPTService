using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Paste
{
	internal class InnerTextFormatDigRichDataFilter : DigRichDataFilter
	{
		public override bool Handle(DataObjectContext dataObject)
		{
			string saveInfoString = dataObject.SaveInfoString;
			if (string.IsNullOrEmpty(dataObject.SaveInfoString))
			{
				return false;
			}
			RichTextSaveInfo richTextSaveInfo = TextContext.StorageParser.Parse(saveInfoString) as RichTextSaveInfo;
			if (richTextSaveInfo != null)
			{
				dataObject.RichTextProperty = richTextSaveInfo.ToRichTextProperty();
				return true;
			}
			return false;
		}
	}
}
