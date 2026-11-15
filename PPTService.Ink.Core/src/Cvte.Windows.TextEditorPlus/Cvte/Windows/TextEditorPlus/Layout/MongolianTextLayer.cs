using System.Collections.Generic;
using System.Windows;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class MongolianTextLayer : TextLayerBase
	{
		internal MongolianTextLayer()
			: base(KnownLayer.Text)
		{
		}

		public override void LayoutLineVisuals(List<ParagraphLayoutData> paragraphLayoutDatas, double renderWidth)
		{
			base.LayoutLineVisuals(paragraphLayoutDatas, renderWidth);
			foreach (ParagraphLayoutData paragraphLayoutData in paragraphLayoutDatas)
			{
				double num = 0.0;
				double visualTop = paragraphLayoutData.VisualTop;
				foreach (LineVisualBase line in paragraphLayoutData.Lines)
				{
					if (!line.IsAlive)
					{
						AddVisualChild(line);
					}
					Visuals.Add(line);
					line.Arrange(new Rect(new Point(visualTop + num, line.StartArrangeOffset), default(Size)));
					num += line.Width;
				}
			}
		}
	}
}
