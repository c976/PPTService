using System;
using System.Collections.Generic;
using System.Windows;
using PPTService.Ink.Core.ComponentModel;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class HorizontalTextLayer : TextLayerBase
	{
		internal HorizontalTextLayer()
			: base(KnownLayer.Text)
		{
		}

		public override void LayoutLineVisuals(List<ParagraphLayoutData> paragraphLayoutDatas, double editorWidth)
		{
			TextTracer.Debug("对文本行进行渲染布局", "文本渲染");
			base.LayoutLineVisuals(paragraphLayoutDatas, editorWidth);
			TextTracer.Debug("将文本行加入视觉树", "文本渲染");
			for (int i = 0; i < paragraphLayoutDatas.Count; i++)
			{
				double num = ((i == 0) ? 0.0 : paragraphLayoutDatas[i].ParagraphBefore);
				double num2 = 0.0;
				foreach (LineVisualBase line in paragraphLayoutDatas[i].Lines)
				{
					if (!line.IsAlive)
					{
						if (line.IsDisposed)
						{
							if (DebuggingProperties.IsDebug)
							{
								throw new ArgumentException("不能传入被释放的 Line 文本行，仅调试下抛出，请找德熙");
							}
							continue;
						}
						AddVisualChild(line);
					}
					Visuals.Add(line);
					line.Arrange(new Rect(new Point(line.StartArrangeOffset, paragraphLayoutDatas[i].VisualTop + num2 + num), default(Size)));
					num2 += line.Height;
				}
			}
		}
	}
}
