using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Caret
{
	public class CaretOffsetTransaction
	{
		private readonly CaretOffset _oldCaretOffset;

		private readonly TextEditor _textEditor;

		internal CaretOffsetTransaction(TextEditor textEditor, CaretOffset caretOffset)
		{
			_textEditor = textEditor;
			_oldCaretOffset = caretOffset;
		}

		public CaretOffsetOperation Commit()
		{
			CaretOffset currentCaretOffset = _textEditor.CaretInfo.CurrentCaretOffset;
			return new CaretOffsetOperation(_textEditor, _oldCaretOffset, currentCaretOffset);
		}
	}
}
