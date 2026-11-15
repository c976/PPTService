using PPTService.Ink.Core.Documents;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Caret
{
	public class CaretOffsetOperation : IOperation
	{
		private readonly CaretOffset _oldCaretOffset;

		private readonly CaretOffset _newCaretOffset;

		private readonly TextEditor _textEditor;

		internal CaretOffsetOperation(TextEditor editor, CaretOffset oldCaretOffset, CaretOffset newCaretOffset)
		{
			_textEditor = editor;
			_oldCaretOffset = oldCaretOffset;
			_newCaretOffset = newCaretOffset;
		}

		public void Undo()
		{
			_textEditor.CaretInfo.CurrentCaretOffset = _oldCaretOffset;
		}

		public void Redo()
		{
			_textEditor.CaretInfo.CurrentCaretOffset = _newCaretOffset;
		}
	}
}
