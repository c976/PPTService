using System.Collections.Generic;
using System.Linq;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Editing;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class HyperlinkHelper
	{
		public static bool CanRemoveHyperlink(TextArea textArea)
		{
			return GetSelectionHyperlinkIndexes(textArea).Any();
		}

		public static bool CanEditHyperlink(TextArea textArea)
		{
			if (textArea.DocumentManager.CurrentFeature.HasFeature(Features.ChangeHyperlinkInner))
			{
				return GetSelectionHyperlinkIndexes(textArea).SelectMany((List<int> x) => x.ToArray()).Count() > 0;
			}
			return false;
		}

		public static bool CanRaiseHyperlink(TextArea textArea)
		{
			if (textArea.DocumentManager.CurrentFeature.HasFeature(Features.HyperlinkFeatureInner))
			{
				return GetSelectionHyperlinkIndexes(textArea).Count((List<int> x) => x.Any()) == 1;
			}
			return false;
		}

		public static bool CanAddHyperlink(TextArea textArea)
		{
			if (textArea.DocumentManager.CurrentFeature.HasFeature(Features.ChangeHyperlinkInner))
			{
				int num = GetSelectionHyperlinkIndexes(textArea).SelectMany((List<int> x) => x.ToArray()).Count();
				bool flag = IsAllExtensionalChars(textArea.DocumentManager);
				if (!textArea.DocumentManager.Selection.IsEmpty && num == 0)
				{
					return !flag;
				}
				return false;
			}
			return false;
		}

		public static bool IsAllExtensionalChars(DocumentManager documentManager)
		{
			ISelection selection = documentManager.Selection;
			if (selection.IsEmpty)
			{
				return false;
			}
			int i = selection.FrontOffset.Offset + 1;
			int num;
			for (num = selection.BehindOffset.Offset + 1; i < num && IsExtensionalChar(documentManager.CharsManager, i); i++)
			{
			}
			if (!selection.IsEmpty && i == num)
			{
				return true;
			}
			return false;
		}

		public static void ResetHyperlinkFace(TextArea textArea)
		{
			bool flag = false;
			foreach (RunProperty allRunProperty in textArea.DocumentManager.PropertyManager.GetAllRunProperties())
			{
				if (allRunProperty.Hyperlink != null)
				{
					flag = true;
					allRunProperty.IsHyperlinkHit = false;
				}
			}
			if (flag)
			{
				textArea.RenderManager.Redraw();
			}
		}

		internal static List<List<int>> GetSelectionHyperlinkIndexes(TextArea textArea)
		{
			List<List<int>> list = new List<List<int>>();
			if (textArea.DocumentManager.Selection.IsEmpty)
			{
				List<int> hyperlinkIndexesByCaretOffset = GetHyperlinkIndexesByCaretOffset(textArea.DocumentManager, textArea.DocumentManager.CaretInfo.CurrentCaretOffset);
				if (hyperlinkIndexesByCaretOffset.Any())
				{
					list.Add(hyperlinkIndexesByCaretOffset);
				}
			}
			else
			{
				int num = textArea.DocumentManager.Selection.FrontOffset.Offset + 1;
				int num2 = textArea.DocumentManager.Selection.BehindOffset.Offset + 1;
				string text = null;
				List<int> list2 = new List<int>();
				for (int i = num; i < num2; i++)
				{
					string hyperlink = textArea.DocumentManager.PropertyManager.GetRunProperty(i).Hyperlink;
					if (hyperlink != null && (hyperlink.Equals(text) || text == null))
					{
						list2.Add(i);
					}
					else if (list2.Any())
					{
						list.Add(list2);
						list2 = new List<int>();
					}
					text = hyperlink;
				}
				if (list2.Any())
				{
					list.Add(list2);
				}
			}
			return list;
		}

		internal static List<int> GetHyperlinkIndexesByCaretOffset(DocumentManager documentManager, CaretOffset caretOffset)
		{
			List<int> list = new List<int>();
			string text = null;
			for (int i = caretOffset.Offset; i < documentManager.PropertyManager.RunPropertiesCount; i++)
			{
				string hyperlink = documentManager.PropertyManager.GetRunProperty(i).Hyperlink;
				if (hyperlink == null || (text != null && !Converter.AreEquals(text, hyperlink)))
				{
					break;
				}
				if (!IsExtensionalChar(documentManager.CharsManager, i))
				{
					list.Add(i);
					text = hyperlink;
				}
			}
			if (!list.Any())
			{
				return list;
			}
			for (int num = caretOffset.Offset - 1; num >= 0; num--)
			{
				string hyperlink2 = documentManager.PropertyManager.GetRunProperty(num).Hyperlink;
				if (hyperlink2 == null || (text != null && !Converter.AreEquals(text, hyperlink2)))
				{
					break;
				}
				if (!IsExtensionalChar(documentManager.CharsManager, num))
				{
					list.Insert(0, num);
					text = hyperlink2;
				}
			}
			return list;
		}

		private static bool IsExtensionalChar(CharsManager charsManager, int index)
		{
			return charsManager.GetCharAt(index) is ExtensionalChar;
		}
	}
}
