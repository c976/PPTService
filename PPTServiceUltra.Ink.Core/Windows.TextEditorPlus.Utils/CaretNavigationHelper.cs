using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Editing;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class CaretNavigationHelper
	{
		private const string RegexStr = "[\\p{P}|\\s]";

		public static CaretOffset GetNewCaretOffset(TextArea textArea, CaretMoveType caretMoveType)
		{
			switch (caretMoveType)
			{
			case CaretMoveType.Left:
				return GetLeftCaretOffset(textArea);
			case CaretMoveType.Right:
				return GetRightCaretOffset(textArea);
			case CaretMoveType.Up:
				return GetUpCaretOffset(textArea);
			case CaretMoveType.Down:
				return GetDownCaretOffset(textArea);
			case CaretMoveType.ControlLeft:
				return GetCtrlLeftCaretOffset(textArea);
			case CaretMoveType.ControlRight:
				return GetCtrlRightCaretOffset(textArea);
			case CaretMoveType.ControlUp:
				return GetCtrlUpCaretOffset(textArea);
			case CaretMoveType.ControlDown:
				return GetCtrlDownCaretOffset(textArea);
			case CaretMoveType.LeftByCharacter:
				return GetPrevCharacterCaretOffset(textArea);
			case CaretMoveType.RightByCharacter:
				return GetNextCharacterCaretOffset(textArea);
			case CaretMoveType.LeftByWord:
				return GetPrevWordCaretOffset(textArea);
			case CaretMoveType.RightByWord:
				return GetNextCharacterCaretOffset(textArea);
			case CaretMoveType.UpByLine:
				return GetPrevLineCaretOffset(textArea);
			case CaretMoveType.DownByLine:
				return GetNextLineCaretOffset(textArea);
			case CaretMoveType.LineStart:
				return GetLineStartCaretOffset(textArea);
			case CaretMoveType.LineEnd:
				return GetLineEndCaretOffset(textArea);
			case CaretMoveType.DocumentStart:
				return GetDocumentStartCaretOffset(textArea);
			case CaretMoveType.DocumentEnd:
				return GetDocumentEndCaretOffset(textArea);
			default:
				return CaretOffset.Default;
			}
		}

		private static CaretOffset GetLeftCaretOffset(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				if (!IsCurrentParagraphDirectionRightToLeft(textArea))
				{
					return GetPrevCharacterCaretOffset(textArea);
				}
				return GetNextCharacterCaretOffset(textArea);
			case ArrangingType.Vertical:
				return GetNextLineCaretOffset(textArea);
			case ArrangingType.Mongolian:
				return GetPrevLineCaretOffset(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingType is not validate");
			}
		}

		private static CaretOffset GetRightCaretOffset(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				if (!IsCurrentParagraphDirectionRightToLeft(textArea))
				{
					return GetNextCharacterCaretOffset(textArea);
				}
				return GetPrevCharacterCaretOffset(textArea);
			case ArrangingType.Vertical:
				return GetPrevLineCaretOffset(textArea);
			case ArrangingType.Mongolian:
				return GetNextLineCaretOffset(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingType is not validate");
			}
		}

		private static CaretOffset GetUpCaretOffset(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return GetPrevLineCaretOffset(textArea);
			case ArrangingType.Vertical:
			case ArrangingType.Mongolian:
				if (!IsCurrentParagraphDirectionRightToLeft(textArea))
				{
					return GetPrevCharacterCaretOffset(textArea);
				}
				return GetNextCharacterCaretOffset(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingType is not validate");
			}
		}

		private static CaretOffset GetDownCaretOffset(TextArea textArea)
		{
			switch (textArea.DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return GetNextLineCaretOffset(textArea);
			case ArrangingType.Vertical:
			case ArrangingType.Mongolian:
				if (!IsCurrentParagraphDirectionRightToLeft(textArea))
				{
					return GetNextCharacterCaretOffset(textArea);
				}
				return GetPrevCharacterCaretOffset(textArea);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", textArea.DocumentManager.ArrangingType, "the arrangingType is not validate");
			}
		}

		private static CaretOffset GetCtrlLeftCaretOffset(TextArea textArea)
		{
			if (textArea.DocumentManager.ArrangingType == ArrangingType.Horizontal)
			{
				if (!IsCurrentParagraphDirectionRightToLeft(textArea))
				{
					return GetPrevWordCaretOffset(textArea);
				}
				return GetNextWordCaretOffset(textArea);
			}
			return textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
		}

		private static CaretOffset GetCtrlRightCaretOffset(TextArea textArea)
		{
			if (textArea.DocumentManager.ArrangingType == ArrangingType.Horizontal)
			{
				if (!IsCurrentParagraphDirectionRightToLeft(textArea))
				{
					return GetNextWordCaretOffset(textArea);
				}
				return GetPrevWordCaretOffset(textArea);
			}
			return textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
		}

		private static CaretOffset GetCtrlUpCaretOffset(TextArea textArea)
		{
			ArrangingType arrangingType = textArea.DocumentManager.ArrangingType;
			if ((uint)(arrangingType - 1) <= 1u)
			{
				if (!IsCurrentParagraphDirectionRightToLeft(textArea))
				{
					return GetPrevWordCaretOffset(textArea);
				}
				return GetNextWordCaretOffset(textArea);
			}
			return textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
		}

		private static CaretOffset GetCtrlDownCaretOffset(TextArea textArea)
		{
			ArrangingType arrangingType = textArea.DocumentManager.ArrangingType;
			if ((uint)(arrangingType - 1) <= 1u)
			{
				if (!IsCurrentParagraphDirectionRightToLeft(textArea))
				{
					return GetNextWordCaretOffset(textArea);
				}
				return GetPrevWordCaretOffset(textArea);
			}
			return textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
		}

		internal static CaretOffset GetLineStartCaretOffset(TextArea textArea)
		{
			DocumentManager documentManager = textArea.DocumentManager;
			ParagraphBase paragraphByCaretOffset = textArea.RenderManager.GetParagraphByCaretOffset(documentManager.CaretInfo.CurrentCaretOffset);
			LineRenderData lineRenderDataByCaretOffset = paragraphByCaretOffset.GetLineRenderDataByCaretOffset(documentManager.CaretInfo.CurrentCaretOffset);
			int lineIndex = paragraphByCaretOffset.GetLineIndex(lineRenderDataByCaretOffset);
			CaretOffset result = default(CaretOffset);
			if (lineIndex == 0)
			{
				result.IsAtParagraphStart = true;
			}
			else
			{
				result.IsAtLineStart = true;
			}
			result.Offset = paragraphByCaretOffset.ParagraphNode.StartOffset - 1 + paragraphByCaretOffset.GetParagraphOffset(lineRenderDataByCaretOffset);
			return result;
		}

		internal static CaretOffset GetLineEndCaretOffset(TextArea textArea)
		{
			DocumentManager documentManager = textArea.DocumentManager;
			ParagraphBase paragraphByCaretOffset = textArea.RenderManager.GetParagraphByCaretOffset(documentManager.CaretInfo.CurrentCaretOffset);
			if (paragraphByCaretOffset.ParagraphNode.Length == 0)
			{
				return documentManager.CaretInfo.CurrentCaretOffset;
			}
			LineRenderData lineRenderDataByCaretOffset = paragraphByCaretOffset.GetLineRenderDataByCaretOffset(documentManager.CaretInfo.CurrentCaretOffset);
			int lineIndex = paragraphByCaretOffset.GetLineIndex(lineRenderDataByCaretOffset);
			CaretOffset result = default(CaretOffset);
			if (lineIndex == paragraphByCaretOffset.LineRenderData.Count - 1)
			{
				result.Offset = paragraphByCaretOffset.ParagraphNode.EndOffset - 1;
			}
			else
			{
				result.Offset = paragraphByCaretOffset.ParagraphNode.StartOffset + paragraphByCaretOffset.Lines[lineIndex].LineProperty.ParagraphOffset + lineRenderDataByCaretOffset.CharVisualsCount - 1;
			}
			return result;
		}

		internal static CaretOffset GetDocumentStartCaretOffset(TextArea textArea)
		{
			return CaretOffset.Default;
		}

		internal static CaretOffset GetDocumentEndCaretOffset(TextArea textArea)
		{
			CaretOffset result = new CaretOffset
			{
				Offset = textArea.DocumentManager.CharsManager.CharCount - 1
			};
			if (textArea.DocumentManager.CharsManager.ParagraphTree.Last().Length == 0)
			{
				result.IsAtParagraphStart = true;
			}
			return result;
		}

		internal static CaretOffset GetPrevCharacterCaretOffset(TextArea textArea)
		{
			DocumentManager documentManager = textArea.DocumentManager;
			if (!documentManager.Selection.IsEmpty && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
			{
				return documentManager.Selection.FrontOffset;
			}
			CaretOffset currentCaretOffset = documentManager.CaretInfo.CurrentCaretOffset;
			ParagraphNode paragraphNodeByDocumentOffset = documentManager.CharsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(currentCaretOffset.Offset + 1);
			CaretOffset result = new CaretOffset
			{
				IsAtParagraphStart = false
			};
			int num = Math.Max(-1, currentCaretOffset.Offset - ((!currentCaretOffset.IsAtParagraphStart) ? 1 : 2));
			if (num == paragraphNodeByDocumentOffset.StartOffset - 1 || (currentCaretOffset.IsAtParagraphStart && paragraphNodeByDocumentOffset.PreviousNode != null && paragraphNodeByDocumentOffset.PreviousNode.Length == 0))
			{
				result.IsAtParagraphStart = true;
			}
			ParagraphBase paragraphByParagraphNumber = textArea.RenderManager.GetParagraphByParagraphNumber(paragraphNodeByDocumentOffset.ParagraphNumber);
			LineRenderData lineRenderDataByCaretOffset = paragraphByParagraphNumber.GetLineRenderDataByCaretOffset(currentCaretOffset);
			if (IsAtLineStart(paragraphByParagraphNumber, lineRenderDataByCaretOffset, num - paragraphNodeByDocumentOffset.StartOffset))
			{
				result.IsAtLineStart = true;
			}
			result.Offset = num;
			return result;
		}

		internal static CaretOffset GetNextCharacterCaretOffset(TextArea textArea)
		{
			DocumentManager documentManager = textArea.DocumentManager;
			if (!documentManager.Selection.IsEmpty && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
			{
				return documentManager.Selection.BehindOffset;
			}
			RenderManager renderManager = textArea.RenderManager;
			CaretOffset currentCaretOffset = documentManager.CaretInfo.CurrentCaretOffset;
			ParagraphNode paragraphNodeByDocumentOffset = documentManager.CharsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(currentCaretOffset.Offset + 1);
			CaretOffset result = new CaretOffset
			{
				IsAtParagraphStart = false
			};
			int num = currentCaretOffset.Offset + 1;
			if (currentCaretOffset.Offset == paragraphNodeByDocumentOffset.EndOffset - 1 && paragraphNodeByDocumentOffset.NextNode != null)
			{
				num++;
				result.IsAtParagraphStart = true;
			}
			if (paragraphNodeByDocumentOffset.NextNode == null && paragraphNodeByDocumentOffset.Length == 0)
			{
				result.IsAtParagraphStart = true;
			}
			num = Math.Min(num, documentManager.CharsManager.CharCount - 1);
			ParagraphBase paragraphByParagraphNumber = renderManager.GetParagraphByParagraphNumber(paragraphNodeByDocumentOffset.ParagraphNumber);
			LineRenderData lineRenderDataByCaretOffset = paragraphByParagraphNumber.GetLineRenderDataByCaretOffset(currentCaretOffset);
			if (IsAtLineEnd(paragraphByParagraphNumber, lineRenderDataByCaretOffset, num - paragraphNodeByDocumentOffset.StartOffset))
			{
				result.IsAtLineStart = true;
			}
			result.Offset = num;
			return result;
		}

		internal static CaretOffset GetPrevWordCaretOffset(TextArea textArea)
		{
			CaretOffset currentCaretOffset = textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
			return GetPrevWordOffset(textArea.DocumentManager.CharsManager.GetText(0, currentCaretOffset.Offset + 1), currentCaretOffset);
		}

		internal static CaretOffset GetNextWordCaretOffset(TextArea textArea)
		{
			CaretOffset currentCaretOffset = textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
			return GetNextWordOffset(textArea.DocumentManager.CharsManager.GetText(currentCaretOffset.Offset + 1, textArea.DocumentManager.CharsManager.CharCount - 1 - currentCaretOffset.Offset), currentCaretOffset);
		}

		internal static CaretOffset GetPrevLineCaretOffset(TextArea textArea, bool isSelection = false)
		{
			DocumentManager documentManager = textArea.DocumentManager;
			RenderManager renderManager = textArea.RenderManager;
			CaretOffset currentCaretOffset = documentManager.CaretInfo.CurrentCaretOffset;
			ParagraphBase paragraphByCaretOffset = renderManager.GetParagraphByCaretOffset(currentCaretOffset);
			LineRenderData lineRenderDataByCaretOffset = paragraphByCaretOffset.GetLineRenderDataByCaretOffset(currentCaretOffset);
			int lineIndex = paragraphByCaretOffset.GetLineIndex(lineRenderDataByCaretOffset);
			ParagraphBase paragraphBase = paragraphByCaretOffset;
			LineRenderData targetLine;
			if (lineIndex == 0)
			{
				int paragraphNumber = paragraphByCaretOffset.ParagraphNode.ParagraphNumber;
				if (paragraphNumber == 1)
				{
					if (!isSelection)
					{
						return currentCaretOffset;
					}
					return CaretOffset.Default;
				}
				paragraphBase = renderManager.GetParagraphByParagraphNumber(paragraphNumber - 1);
				targetLine = paragraphBase.LineRenderData.Last();
			}
			else
			{
				targetLine = paragraphBase.LineRenderData[lineIndex - 1];
			}
			return CalcTargetCaret(textArea, paragraphByCaretOffset, paragraphBase, targetLine);
		}

		internal static CaretOffset GetNextLineCaretOffset(TextArea textArea, bool isSelection = false)
		{
			DocumentManager documentManager = textArea.DocumentManager;
			RenderManager renderManager = textArea.RenderManager;
			CaretOffset currentCaretOffset = documentManager.CaretInfo.CurrentCaretOffset;
			ParagraphBase paragraphByCaretOffset = renderManager.GetParagraphByCaretOffset(currentCaretOffset);
			LineRenderData lineRenderDataByCaretOffset = paragraphByCaretOffset.GetLineRenderDataByCaretOffset(currentCaretOffset);
			int lineIndex = paragraphByCaretOffset.GetLineIndex(lineRenderDataByCaretOffset);
			ParagraphBase paragraphBase = paragraphByCaretOffset;
			LineRenderData targetLine;
			if (lineIndex < paragraphBase.LineRenderData.Count - 1)
			{
				targetLine = paragraphBase.LineRenderData[lineIndex + 1];
			}
			else
			{
				if (paragraphByCaretOffset.ParagraphNode.NextNode == null)
				{
					if (!isSelection)
					{
						return currentCaretOffset;
					}
					return new CaretOffset
					{
						Offset = documentManager.CharsManager.CharCount - 1
					};
				}
				int paragraphNumber = paragraphByCaretOffset.ParagraphNode.ParagraphNumber;
				paragraphBase = renderManager.GetParagraphByParagraphNumber(paragraphNumber + 1);
				targetLine = paragraphBase.LineRenderData.First();
			}
			return CalcTargetCaret(textArea, paragraphByCaretOffset, paragraphBase, targetLine);
		}

		internal static CaretOffset GetPrevWordOffset(string text, CaretOffset currentOffset)
		{
			CaretOffset result = default(CaretOffset);
			Match match = new Regex("[\\p{P}|\\s]", RegexOptions.RightToLeft).Match(text);
			if (!string.IsNullOrEmpty(match.ToString()))
			{
				int val = text.LastIndexOf(match.ToString(), StringComparison.CurrentCulture);
				int val2 = ((currentOffset.IsAtParagraphStart && currentOffset.Offset != -1) ? (currentOffset.Offset - 2) : (currentOffset.Offset - 1));
				result.Offset = Math.Min(val, val2);
				result.IsAtParagraphStart = text.Substring(result.Offset).StartsWith("\n");
			}
			else
			{
				result.Offset = -1;
			}
			return result;
		}

		internal static CaretOffset GetNextWordOffset(string text, CaretOffset currentOffset)
		{
			CaretOffset result = new CaretOffset
			{
				IsAtParagraphStart = text.StartsWith("\r\n")
			};
			Match match = new Regex("[\\p{P}|\\s]").Match(text);
			if (!string.IsNullOrEmpty(match.ToString()))
			{
				int val = text.IndexOf(match.ToString(), StringComparison.CurrentCulture);
				int val2 = ((!text.StartsWith("\r\n")) ? 1 : 2);
				result.Offset = currentOffset.Offset + Math.Max(val2, val);
			}
			else
			{
				result.Offset = currentOffset.Offset + text.Length;
			}
			return result;
		}

		private static bool IsAtLineEnd(ParagraphBase paragraph, LineRenderData line, int offset)
		{
			int lineIndex = paragraph.GetLineIndex(line);
			LineProperty lineProperty = paragraph.Lines[lineIndex].LineProperty;
			if (lineIndex != paragraph.LineRenderData.Count - 1)
			{
				return lineProperty.ParagraphOffset + lineProperty.Length - 1 == offset;
			}
			return false;
		}

		private static bool IsAtLineStart(ParagraphBase paragraph, LineRenderData line, int offset)
		{
			int lineIndex = paragraph.GetLineIndex(line);
			LineProperty lineProperty = paragraph.Lines[lineIndex].LineProperty;
			if (lineIndex != 0)
			{
				return lineProperty.ParagraphOffset - 1 == offset;
			}
			return false;
		}

		private static CaretOffset CalcTargetCaret(TextArea textArea, ParagraphBase currentParagraph, ParagraphBase targetParagraph, LineRenderData targetLine)
		{
			CaretOffset currentCaretOffset = textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
			double x;
			double y;
			if (textArea.DocumentManager.ArrangingType.Equals(ArrangingType.Horizontal))
			{
				x = currentParagraph.GetVisualXPositionByCaretOffset(currentCaretOffset);
				y = targetParagraph.GetLineParagraphYPosition(targetLine);
			}
			else
			{
				x = targetParagraph.GetLineParagraphYPosition(targetLine);
				y = currentParagraph.GetVisualYPositionByCaretOffset(currentCaretOffset);
			}
			return targetParagraph.GetCaretOffsetByParagraphPosition(new Point(x, y));
		}

		private static bool IsCurrentParagraphDirectionRightToLeft(TextArea textArea)
		{
			return textArea.DocumentManager.PropertyManager.CurrentParagraphProperty.Direction == FlowDirection.RightToLeft;
		}
	}
}
