using System.Linq;
using System.Windows;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class MongolianParagraph : VerticalParagraphBase
	{
		internal MongolianParagraph(CharsManager charsManager, PropertyManager propertyManager, ParagraphNode paragraphNode, ITextGeometryRender textGeometryRender)
			: base(charsManager, propertyManager, paragraphNode, textGeometryRender)
		{
		}

		internal override Rect GetRunBoundsByCaretOffset(CaretOffset caretOffset)
		{
			int num = caretOffset.Offset - ParagraphNode.StartOffset;
			LineRenderData lineRenderDataByCaretOffset = GetLineRenderDataByCaretOffset(caretOffset);
			int lineIndex = GetLineIndex(lineRenderDataByCaretOffset);
			double num2 = ((lineIndex == 0) ? ParagraphTextBeforeData.FirstLineTextIndent : ParagraphTextBeforeData.NotFirstLineTextIndent);
			double num3 = num2;
			if (lineRenderDataByCaretOffset.CharVisuals.Count > 0)
			{
				ICharVisual charVisual = lineRenderDataByCaretOffset.CharVisuals[0];
				Rect bounds = charVisual.Bounds;
				num3 += ((charVisual.ActualDirection == FlowDirection.LeftToRight) ? bounds.Top : bounds.Bottom);
			}
			else
			{
				num3 += Lines[lineIndex].DefaultRunBounds.Y;
			}
			double num4 = LineRenderData.GetRange(0, lineIndex).Sum((LineRenderData x) => x.Height);
			if (caretOffset.IsAtParagraphStart)
			{
				Rect defaultRunBounds = Lines[lineIndex].DefaultRunBounds;
				return new Rect(defaultRunBounds.X + num4, num3, defaultRunBounds.Width, defaultRunBounds.Height);
			}
			if (caretOffset.IsAtLineStart)
			{
				Rect defaultRunBounds2 = Lines[lineIndex].DefaultRunBounds;
				return new Rect(defaultRunBounds2.X + num4, num3, defaultRunBounds2.Width, defaultRunBounds2.Height);
			}
			int index = num - Lines[lineIndex].LineProperty.ParagraphOffset;
			return lineRenderDataByCaretOffset.CharVisuals[index].Bounds.OffsetRect(num4, num2);
		}
	}
}
