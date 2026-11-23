namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Paste
{
	internal class HtmlDigRichDataFilter : DigRichDataFilter
	{
		public override bool Handle(DataObjectContext dataObject)
		{
			string.IsNullOrEmpty(dataObject.Html);
			return false;
		}
	}
}
