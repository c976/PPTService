using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus
{
	public class RenderInfo
	{
		private RenderManager RenderManager { get; }

		private TextEditor TextEditor { get; }

		internal RenderInfo(TextEditor textEditor)
		{
			TextEditor = textEditor;
			RenderManager = textEditor.RenderManager;
		}

		public List<ParagraphLinesInfo> GetDocumentParagraphLinesInfoList()
		{
			List<ParagraphLinesInfo> list = new List<ParagraphLinesInfo>(RenderManager.Paragraphs.Count);
			for (int i = 0; i < RenderManager.Paragraphs.Count; i++)
			{
				ParagraphLinesInfo item = new ParagraphLinesInfo(RenderManager.Paragraphs[i].ParagraphLineInfoRenderedData);
				list.Add(item);
			}
			return list;
		}
	}
}
