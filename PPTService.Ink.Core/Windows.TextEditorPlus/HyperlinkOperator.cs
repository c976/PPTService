using System.Collections.Generic;
using System.Linq;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Editing;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus
{
	public class HyperlinkOperator
	{
		private readonly TextArea _textArea;

		public HyperlinkOperator(TextArea textArea)
		{
			_textArea = textArea;
		}

		public bool CanRemoveHyperlink()
		{
			return GetSelectionHyperlinkIndexs().SelectMany((List<int> x) => x.ToArray()).Count() > 0;
		}

		public bool CanEditHyperlink()
		{
			return HyperlinkHelper.CanEditHyperlink(_textArea);
		}

		public bool CanRaiseHyperlink()
		{
			return HyperlinkHelper.CanRaiseHyperlink(_textArea);
		}

		public bool CanAddHyperlink()
		{
			return HyperlinkHelper.CanAddHyperlink(_textArea);
		}

		public void ResetHyperlinkFace()
		{
			HyperlinkHelper.ResetHyperlinkFace(_textArea);
		}

		public List<List<int>> GetSelectionHyperlinkIndexs()
		{
			return HyperlinkHelper.GetSelectionHyperlinkIndexes(_textArea);
		}

		public List<int> GetHyperlinkIndexsByCaretOffset(CaretOffset caretOffset)
		{
			return HyperlinkHelper.GetHyperlinkIndexesByCaretOffset(_textArea.DocumentManager, caretOffset);
		}

		public string GetHyperlinkByCaretOffset(CaretOffset caretOffset)
		{
			return _textArea.DocumentManager.PropertyManager.GetRunProperty(caretOffset.Offset).Hyperlink;
		}

		public void RequestAddHyperlink()
		{
			_textArea.OnHyperlinkAdded();
		}

		public void ReqeustEditHyperlink()
		{
			_textArea.OnHyperlinkEdited();
		}
	}
}
