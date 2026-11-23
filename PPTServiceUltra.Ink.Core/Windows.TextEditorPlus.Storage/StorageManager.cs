using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage
{
	internal class StorageManager
	{
		private readonly DocumentManager _documentManager;

		private readonly TextEditor _textEditor;

		internal StorageManager(TextEditor textEditor)
		{
			_textEditor = textEditor;
			_documentManager = textEditor.DocumentManager;
		}

		internal RichTextSaveInfo GetRichText()
		{
			List<ParagraphSaveInfo> list = new List<ParagraphSaveInfo>();
			StringBuilder strBuilder = new StringBuilder();
			int paragraphCount = _documentManager.CharsManager.ParagraphTree.ParagraphCount;
			if (_textEditor.RenderManager.Paragraphs.Count > 0 && paragraphCount > _textEditor.RenderManager.Paragraphs.Count)
			{
				throw new InvalidOperationException("文本管理的段落数量和渲染的段落数量不相同，请判断在调用 GetRichText 之前文本已经渲染完成。如果此时文本没有渲染完成，请调用 TextEditor.ForceRedraw 渲染");
			}
			for (int i = 0; i < paragraphCount; i++)
			{
				ParagraphSaveInfo paragraphSaveInfo = TextHelperInner.GetParagraphSaveInfo(_documentManager, i);
				if (_textEditor.RenderManager.Paragraphs.Any())
				{
					paragraphSaveInfo.Lines = _textEditor.RenderManager.Paragraphs[i].ParagraphLineInfoRenderedData.Select((LineProperty x) => x.ToLineSaveInfo()).ToList();
				}
				paragraphSaveInfo.TextRuns.ForEach((RunSaveInfo x) =>
				{
					strBuilder.Append(x.Text);
				});
				list.Add(paragraphSaveInfo);
			}
			return new RichTextSaveInfo
			{
				VerticalTextAlignment = _documentManager.TextVerticalAlignment,
				SizeToContent = _documentManager.SizeToContent,
				ArrangingType = _documentManager.ArrangingType,
				Paragraphs = list,
				Text = strBuilder.ToString()
			};
		}

		internal void SetRichText(RichTextSaveInfo richTextSaveInfo, bool withForceRedraw = true)
		{
			FixSaveInfoWrongData(richTextSaveInfo);
			RichTextProperty richTextProperty = richTextSaveInfo.ToRichTextProperty();
			_textEditor.ArrangingType = richTextProperty.ArrangingType;
			_textEditor.SizeToContent = richTextProperty.SizeToContent;
			_textEditor.TextVerticalAlignment = richTextProperty.VerticalTextAlignment;
			_documentManager.PropertyManager.ParagraphLineInfoRenderingData = richTextProperty.ParagraphLineInfo;
			_documentManager.ReplaceSelectionWithRichTextProperty(new SimpleSegment(0, _textEditor.CharCount), richTextProperty);
			_documentManager.ChangeProperty((RunProperty p) =>
			{
				if (!TextContext.CheckFontFamilyInstalled(p.FontFamily.Source))
				{
					p.FontFamily = TextContext.DefaultRenderFontFamily;
				}
			});
			if (withForceRedraw)
			{
				_textEditor.RenderManager.IsRedrawEnabled = true;
				_textEditor.ForceRedraw();
			}
		}

		private void FixSaveInfoWrongData(RichTextSaveInfo info)
		{
			int num = info.Text?.Length ?? 0;
			StorageNullValueRepairman.FixRichTextSaveInfo(info);
			int num2 = info.Paragraphs.Sum((ParagraphSaveInfo p) => p.Lines.Sum((LineSaveInfo l) => l.Length));
			if (num == 0 || num2 == 0 || num <= num2)
			{
				return;
			}
			foreach (ParagraphSaveInfo paragraph in info.Paragraphs)
			{
				paragraph.Lines.Clear();
			}
		}
	}
}
