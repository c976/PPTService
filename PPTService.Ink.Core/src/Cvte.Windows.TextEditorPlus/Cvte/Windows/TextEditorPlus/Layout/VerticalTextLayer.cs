using System.Collections.Generic;
using System.Windows;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class VerticalTextLayer : TextLayerBase
	{
		internal VerticalTextLayer()
			: base(KnownLayer.Text)
		{
		}

		public override void LayoutLineVisuals(List<ParagraphLayoutData> paragraphLayoutDatas, double editorWidth)
		{
			base.LayoutLineVisuals(paragraphLayoutDatas, editorWidth);
			foreach (ParagraphLayoutData paragraphLayoutData in paragraphLayoutDatas)
			{
				double num = 0.0;
				double num2 = editorWidth - paragraphLayoutData.VisualTop;
				foreach (LineVisualBase line in paragraphLayoutData.Lines)
				{
					if (!line.IsAlive)
					{
						AddVisualChild(line);
					}
					Visuals.Add(line);
					line.Arrange(new Rect(new Point(num2 - line.Width - num, line.StartArrangeOffset), default(Size)));
					num += line.Width;
				}
			}
		}
	}
}
