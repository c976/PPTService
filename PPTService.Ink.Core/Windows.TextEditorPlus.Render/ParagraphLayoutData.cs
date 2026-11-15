using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class ParagraphLayoutData
	{
		internal double VisualTop { get; set; }

		internal UiElementCharVisual MarkerVisual { get; set; }

		internal List<LineVisualBase> Lines { get; set; }

		internal double ParagraphBefore { get; set; }

		internal double ParagraphAfter { get; set; }
	}
}
