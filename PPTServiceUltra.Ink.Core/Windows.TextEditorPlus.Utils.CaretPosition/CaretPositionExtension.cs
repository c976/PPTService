using System;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.CaretPosition
{
	internal static class CaretPositionExtension
	{
		public static SpecialCaretPosition GetSpecialCaretPositionByCaretOffset(TextEditor editor)
		{
			if (CheckIsEmptyDocument(editor))
			{
				return SpecialCaretPosition.DocumentAll;
			}
			int caretOffset = GetCaretOffset(editor);
			ValueTuple<bool, bool, bool> caretInDocumentLineInfo = GetCaretInDocumentLineInfo(caretOffset, editor);
			bool item = caretInDocumentLineInfo.Item1;
			bool item2 = caretInDocumentLineInfo.Item2;
			bool item3 = caretInDocumentLineInfo.Item3;
			if (item)
			{
				return SpecialCaretPosition.DocumentNone;
			}
			bool flag = CheckCaretInDocumentStart(caretOffset);
			bool flag2 = CheckCaretInDocumentEnd(caretOffset, editor);
			SpecialCaretPosition specialCaretPosition = SpecialCaretPosition.DocumentNone;
			if (item2)
			{
				specialCaretPosition |= SpecialCaretPosition.DocumentTop;
			}
			if (item3)
			{
				specialCaretPosition |= SpecialCaretPosition.DocumentBottom;
			}
			if (flag)
			{
				specialCaretPosition |= SpecialCaretPosition.DocumentStart;
			}
			if (flag2)
			{
				specialCaretPosition |= SpecialCaretPosition.DocumentEnd;
			}
			return specialCaretPosition;
		}

		
		private static ValueTuple<bool, bool, bool> GetCaretInDocumentLineInfo(int caretOffset, TextEditor editor)
		{
			DocumentOffset offset = new DocumentOffset(caretOffset + 1);
			int caretParagraphNumber = GetCaretParagraphNumber(editor.GetParagraphNodeByDocumentOffset(offset));
			bool flag = caretParagraphNumber == 1;
			bool flag2 = caretParagraphNumber == editor.ParagraphCount;
			if (!flag && !flag2)
			{
				return new ValueTuple<bool, bool, bool>(true, false, false);
			}
			ParagraphBase paragraphByParagraphNumber = editor.RenderManager.GetParagraphByParagraphNumber(caretParagraphNumber);
			if (paragraphByParagraphNumber == null)
			{
				throw new InvalidOperationException("应在文档渲染之后，调用该函数");
			}
			LineRenderData lineRenderDataByCaretOffset = paragraphByParagraphNumber.GetLineRenderDataByCaretOffset(editor.CaretInfo.CurrentCaretOffset);
			if (lineRenderDataByCaretOffset == null)
			{
				throw new InvalidOperationException("应在文档渲染之后，调用该函数");
			}
			bool item = false;
			if (flag)
			{
				LineRenderData obj = paragraphByParagraphNumber.LineRenderData.First();
				item = lineRenderDataByCaretOffset.Equals(obj);
			}
			bool item2 = false;
			if (flag2)
			{
				LineRenderData obj2 = paragraphByParagraphNumber.LineRenderData.Last();
				item2 = lineRenderDataByCaretOffset.Equals(obj2);
			}
			return new ValueTuple<bool, bool, bool>(false, item, item2);
		}

		private static bool CheckCaretInDocumentEnd(int caretOffset, TextEditor editor)
		{
			return caretOffset + 1 == editor.CharCount;
		}

		private static bool CheckCaretInDocumentStart(int caretOffset)
		{
			return caretOffset == -1;
		}

		private static bool CheckIsEmptyDocument(TextEditor editor)
		{
			return editor.CharCount == 0;
		}

		private static int GetCaretOffset(TextEditor editor)
		{
			return editor.CaretInfo.CurrentCaretOffset.Offset;
		}

		private static int GetCaretParagraphNumber(IParagraphSegment paragraphNode)
		{
			return paragraphNode.ParagraphNumber;
		}
	}
}
