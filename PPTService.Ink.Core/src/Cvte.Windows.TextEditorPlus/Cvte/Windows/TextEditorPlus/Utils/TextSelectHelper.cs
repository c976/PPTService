using System;
using System.Linq;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Editing;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class TextSelectHelper
	{
		public static ISelection Select(TextArea textArea, SelectionType type)
		{
			switch (type)
			{
			case SelectionType.LeftByCharacter:
				return SelectLeftByCharacter(textArea);
			case SelectionType.RightByCharacter:
				return SelectRightByCharacter(textArea);
			case SelectionType.LeftByWord:
				return SelectLeftByWord(textArea);
			case SelectionType.RightByWord:
				return SelectRightByWord(textArea);
			case SelectionType.UpByLine:
				return SelectUpByLine(textArea);
			case SelectionType.DownByLine:
				return SelectDownByLine(textArea);
			case SelectionType.LineStart:
				return SelectToLineStart(textArea);
			case SelectionType.LineEnd:
				return SelectToLineEnd(textArea);
			case SelectionType.DocumentStart:
				return SelectToDocumentStart(textArea);
			case SelectionType.DocumentEnd:
				return SelectToDocumentEnd(textArea);
			case SelectionType.All:
				return SelectAll(textArea);
			case SelectionType.ShiftLeft:
				return SelectByShiftLeft(textArea);
			case SelectionType.ShiftRight:
				return SelectByShiftRight(textArea);
			case SelectionType.ShiftUp:
				return SelectByShiftUp(textArea);
			case SelectionType.ShiftDown:
				return SelectByShiftDown(textArea);
			case SelectionType.ControlShiftLeft:
				return SelectByControlShiftLeft(textArea);
			case SelectionType.ControlShiftRight:
				return SelectByControlShiftRight(textArea);
			case SelectionType.ControlShiftUp:
				return SelectByControlShiftUp(textArea);
			case SelectionType.ControlShiftDown:
				return SelectByControlShiftDown(textArea);
			default:
				throw new ArgumentOutOfRangeException("type", type, "the selectionType is not validate");
			}
		}

		private static ISelection SelectByShiftLeft(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return SelectLeftByCharacter(textArea);
			case ArrangingType.Vertical:
				return SelectDownByLine(textArea);
			case ArrangingType.Mongolian:
				return SelectUpByLine(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingtype is not validate");
			}
		}

		private static ISelection SelectByShiftRight(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return SelectRightByCharacter(textArea);
			case ArrangingType.Vertical:
				return SelectUpByLine(textArea);
			case ArrangingType.Mongolian:
				return SelectDownByLine(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingtype is not validate");
			}
		}

		private static ISelection SelectByShiftUp(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return SelectUpByLine(textArea);
			case ArrangingType.Vertical:
			case ArrangingType.Mongolian:
				return SelectLeftByCharacter(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingtype is not validate");
			}
		}

		private static ISelection SelectByShiftDown(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return SelectDownByLine(textArea);
			case ArrangingType.Vertical:
			case ArrangingType.Mongolian:
				return SelectRightByCharacter(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingtype is not validate");
			}
		}

		private static ISelection SelectByControlShiftLeft(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return SelectLeftByWord(textArea);
			case ArrangingType.Vertical:
				return SelectDownByLine(textArea);
			case ArrangingType.Mongolian:
				return SelectUpByLine(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingtype is not validate");
			}
		}

		private static ISelection SelectByControlShiftRight(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return SelectRightByWord(textArea);
			case ArrangingType.Vertical:
				return SelectUpByLine(textArea);
			case ArrangingType.Mongolian:
				return SelectDownByLine(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingtype is not validate");
			}
		}

		private static ISelection SelectByControlShiftUp(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return SelectUpByLine(textArea);
			case ArrangingType.Vertical:
			case ArrangingType.Mongolian:
				return SelectLeftByWord(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingtype is not validate");
			}
		}

		private static ISelection SelectByControlShiftDown(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return SelectDownByLine(textArea);
			case ArrangingType.Vertical:
			case ArrangingType.Mongolian:
				return SelectRightByWord(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingtype is not validate");
			}
		}

		internal static ISelection SelectAll(TextArea textArea)
		{
			return new Selection(CaretOffset.Default, GetDocumentEndCaretOffset(textArea));
		}

		internal static ISelection SelectLeftByCharacter(TextArea textArea)
		{
			CaretOffset prevCharacterCaretOffset = CaretNavigationHelper.GetPrevCharacterCaretOffset(textArea);
			return new Selection(GetStartOffset(textArea), prevCharacterCaretOffset);
		}

		internal static ISelection SelectRightByCharacter(TextArea textArea)
		{
			CaretOffset nextCharacterCaretOffset = CaretNavigationHelper.GetNextCharacterCaretOffset(textArea);
			return new Selection(GetStartOffset(textArea), nextCharacterCaretOffset);
		}

		internal static ISelection SelectLeftByWord(TextArea textArea)
		{
			ISelection selection = textArea.DocumentManager.Selection;
			CaretOffset currentCaretOffset = textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
			CaretOffset caretOffset = (selection.IsEmpty ? currentCaretOffset : selection.FrontOffset);
			CaretOffset caretOffset2 = (selection.IsEmpty ? currentCaretOffset : selection.BehindOffset);
			if (textArea.DocumentManager.CaretInfo.CurrentCaretOffset.Offset == -1)
			{
				return selection;
			}
			CaretOffset prevWordOffset = CaretNavigationHelper.GetPrevWordOffset(textArea.DocumentManager.CharsManager.GetText(0, currentCaretOffset.Offset + 1), currentCaretOffset);
			CaretOffset caretOffset3 = caretOffset2.Clone();
			if (currentCaretOffset.Offset == caretOffset2.Offset)
			{
				caretOffset3.Offset = caretOffset.Offset;
			}
			return new Selection(TextHelperInner.UpdateCaretOffsetIsAtParagraphStart(textArea.DocumentManager, caretOffset3), TextHelperInner.UpdateCaretOffsetIsAtParagraphStart(textArea.DocumentManager, prevWordOffset));
		}

		internal static ISelection SelectRightByWord(TextArea textArea)
		{
			ISelection selection = textArea.DocumentManager.Selection;
			CaretOffset currentCaretOffset = textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
			CaretOffset caretOffset = (selection.IsEmpty ? currentCaretOffset : selection.FrontOffset);
			CaretOffset caretOffset2 = (selection.IsEmpty ? currentCaretOffset : selection.BehindOffset);
			if (currentCaretOffset.Offset + 1 == textArea.DocumentManager.CharsManager.CharCount)
			{
				return selection;
			}
			CaretOffset nextWordOffset = CaretNavigationHelper.GetNextWordOffset(textArea.DocumentManager.CharsManager.GetText(currentCaretOffset.Offset + 1, textArea.DocumentManager.CharsManager.CharCount - 1 - currentCaretOffset.Offset), currentCaretOffset);
			CaretOffset caretOffset3 = caretOffset.Clone();
			if (currentCaretOffset.Offset == caretOffset.Offset)
			{
				caretOffset3.Offset = caretOffset2.Offset;
			}
			return new Selection(TextHelperInner.UpdateCaretOffsetIsAtParagraphStart(textArea.DocumentManager, caretOffset3), TextHelperInner.UpdateCaretOffsetIsAtParagraphStart(textArea.DocumentManager, nextWordOffset));
		}

		internal static ISelection SelectUpByLine(TextArea textArea)
		{
			CaretOffset prevLineCaretOffset = CaretNavigationHelper.GetPrevLineCaretOffset(textArea, true);
			return new Selection(GetStartOffset(textArea), prevLineCaretOffset);
		}

		internal static ISelection SelectDownByLine(TextArea textArea)
		{
			CaretOffset nextLineCaretOffset = CaretNavigationHelper.GetNextLineCaretOffset(textArea, true);
			return new Selection(GetStartOffset(textArea), nextLineCaretOffset);
		}

		internal static ISelection SelectToLineStart(TextArea textArea)
		{
			CaretOffset lineStartCaretOffset = CaretNavigationHelper.GetLineStartCaretOffset(textArea);
			return new Selection(GetStartOffset(textArea), lineStartCaretOffset);
		}

		internal static ISelection SelectToLineEnd(TextArea textArea)
		{
			CaretOffset lineEndCaretOffset = CaretNavigationHelper.GetLineEndCaretOffset(textArea);
			return new Selection(GetStartOffset(textArea), lineEndCaretOffset);
		}

		internal static ISelection SelectToDocumentStart(TextArea textArea)
		{
			CaretOffset endOffset = CaretOffset.Default;
			return new Selection(GetStartOffset(textArea), endOffset);
		}

		internal static ISelection SelectToDocumentEnd(TextArea textArea)
		{
			return new Selection(GetStartOffset(textArea), GetDocumentEndCaretOffset(textArea));
		}

		private static CaretOffset GetStartOffset(TextArea textArea)
		{
			ISelection selection = textArea.DocumentManager.Selection;
			CaretOffset currentCaretOffset = textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
			if (!selection.IsEmpty)
			{
				return selection.StartOffset;
			}
			return currentCaretOffset;
		}

		private static CaretOffset GetDocumentEndCaretOffset(TextArea textArea)
		{
			return new CaretOffset
			{
				IsAtLineStart = false,
				Offset = textArea.DocumentManager.CharsManager.CharCount - 1,
				IsAtParagraphStart = (textArea.DocumentManager.CharsManager.ParagraphTree.Last().Length == 0)
			};
		}
	}
}
