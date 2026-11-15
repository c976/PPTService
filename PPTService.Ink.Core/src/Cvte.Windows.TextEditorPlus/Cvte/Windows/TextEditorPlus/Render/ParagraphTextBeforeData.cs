using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class ParagraphTextBeforeData
	{
		internal UiElementCharVisual MarkerCharVisual { get; set; }

		internal ICharObject MarkerChar { get; set; }

		internal double FirstLineTextIndent { get; set; }

		internal double NotFirstLineTextIndent { get; set; }
	}
}
