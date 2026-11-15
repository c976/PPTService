using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Editing
{
	internal class KeyboardHandler
	{
		private readonly TextArea _textArea;

		private readonly DocumentManager _documentManager;

		internal KeyboardHandler(TextArea textArea)
		{
			_textArea = textArea;
			_documentManager = textArea.DocumentManager;
			SetCommands();
			SetInputKeys();
		}

		private void SetCommands()
		{
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveLeftByCharacter, MoveCaret(CaretMoveType.LeftByCharacter)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveRightByCharacter, MoveCaret(CaretMoveType.RightByCharacter)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveLeftByWord, MoveCaret(CaretMoveType.LeftByWord)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveRightByWord, MoveCaret(CaretMoveType.RightByWord)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveUpByLine, MoveCaret(CaretMoveType.UpByLine)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveDownByLine, MoveCaret(CaretMoveType.DownByLine)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveToLineStart, MoveCaret(CaretMoveType.LineStart)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveToLineEnd, MoveCaret(CaretMoveType.LineEnd)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveToDocumentStart, MoveCaret(CaretMoveType.DocumentStart)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.MoveToDocumentEnd, MoveCaret(CaretMoveType.DocumentEnd)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.MoveLeftCommand, MoveCaret(CaretMoveType.Left)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.MoveRightCommand, MoveCaret(CaretMoveType.Right)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.MoveUpCommand, MoveCaret(CaretMoveType.Up)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.MoveDownCommand, MoveCaret(CaretMoveType.Down)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ControlLeftCommand, MoveCaret(CaretMoveType.ControlLeft)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ControlRightCommand, MoveCaret(CaretMoveType.ControlRight)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ControlUpommand, MoveCaret(CaretMoveType.ControlUp)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ControlDownCommand, MoveCaret(CaretMoveType.ControlDown)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ShiftLeftCommand, SelectText(SelectionType.ShiftLeft)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ShiftRightCommand, SelectText(SelectionType.ShiftRight)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ShiftUpommand, SelectText(SelectionType.ShiftUp)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ShiftDownCommand, SelectText(SelectionType.ShiftDown)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ControlShiftLeftCommand, SelectText(SelectionType.ControlShiftLeft)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ControlShiftRightCommand, SelectText(SelectionType.ControlShiftRight)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ControlShiftUpommand, SelectText(SelectionType.ControlShiftUp)));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ControlShiftDownCommand, SelectText(SelectionType.ControlShiftDown)));
			_textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, SelectText(SelectionType.All)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectLeftByCharacter, SelectText(SelectionType.LeftByCharacter)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectRightByCharacter, SelectText(SelectionType.RightByCharacter)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectLeftByWord, SelectText(SelectionType.LeftByWord)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectRightByWord, SelectText(SelectionType.RightByWord)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectUpByLine, SelectText(SelectionType.UpByLine)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectDownByLine, SelectText(SelectionType.DownByLine)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectToLineStart, SelectText(SelectionType.LineStart)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectToLineEnd, SelectText(SelectionType.LineEnd)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectToDocumentStart, SelectText(SelectionType.DocumentStart)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.SelectToDocumentEnd, SelectText(SelectionType.DocumentEnd)));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.Backspace, OnDeleteCharacter));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.Delete, OnDelete));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.AttachingEnterCommand, OnEnter));
			_textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, CanCopy));
			_textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, CanCut));
			_textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.DeleteNextWord, OnDeleteNextWord));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.DeletePreviousWord, OnDeletePreviousWord));
			_textArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.PastePlainTextCommand, OnPastePlainText));
			_textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUndo, CanUndo));
			_textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, OnRedo, CanRedo));
			_textArea.CommandBindings.Add(new CommandBinding(EditingCommands.TabForward, OnTabForward));
		}

		private void SetInputKeys()
		{
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.MoveLeftCommand, Key.Left, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.MoveRightCommand, Key.Right, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.MoveUpCommand, Key.Up, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.MoveDownCommand, Key.Down, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ControlLeftCommand, Key.Left, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ControlRightCommand, Key.Right, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ControlUpommand, Key.Up, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ControlDownCommand, Key.Down, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ShiftLeftCommand, Key.Left, ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ShiftRightCommand, Key.Right, ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ShiftUpommand, Key.Up, ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ShiftDownCommand, Key.Down, ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ControlShiftLeftCommand, Key.Left, ModifierKeys.Control | ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ControlShiftRightCommand, Key.Right, ModifierKeys.Control | ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ControlShiftUpommand, Key.Up, ModifierKeys.Control | ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.ControlShiftDownCommand, Key.Down, ModifierKeys.Control | ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.MoveToLineStart, Key.Home, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.MoveToLineEnd, Key.End, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.MoveToDocumentStart, Key.Home, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.MoveToDocumentEnd, Key.End, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.Backspace, Key.Back, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.Delete, Key.Delete, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.SelectToLineStart, Key.Home, ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.SelectToLineEnd, Key.End, ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.SelectToDocumentStart, Key.Home, ModifierKeys.Control | ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.SelectToDocumentEnd, Key.End, ModifierKeys.Control | ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.PastePlainTextCommand, Key.V, ModifierKeys.Control | ModifierKeys.Shift));
			_textArea.InputBindings.Add(new KeyBinding(ApplicationCommands.Paste, Key.V, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(ApplicationCommands.Copy, Key.C, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(ApplicationCommands.Cut, Key.X, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.AttachingEnterCommand, Key.Return, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(SetFormatCommands.AttachingEnterCommand, Key.Return, ModifierKeys.Alt));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.TabForward, Key.Tab, ModifierKeys.None));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.DeleteNextWord, Key.Delete, ModifierKeys.Control));
			_textArea.InputBindings.Add(new KeyBinding(EditingCommands.DeletePreviousWord, Key.Back, ModifierKeys.Control));
		}

		private ExecutedRoutedEventHandler MoveCaret(CaretMoveType type)
		{
			return (object o, ExecutedRoutedEventArgs args) =>
			{
				MoveCaretInner(type);
			};
		}

		private void MoveCaretInner(CaretMoveType type)
		{
			if (_documentManager.CurrentFeature.HasFeature(Features.CaretInner))
			{
				CaretOffset newCaretOffset = CaretNavigationHelper.GetNewCaretOffset(_textArea, type);
				_documentManager.MoveCaret(newCaretOffset);
			}
		}

		private ExecutedRoutedEventHandler SelectText(SelectionType type)
		{
			return (object o, ExecutedRoutedEventArgs args) =>
			{
				SelectTextInner(type);
			};
		}

		private void SelectTextInner(SelectionType type)
		{
			if (_documentManager.CurrentFeature.HasFeature(Features.SelectInner))
			{
				ISelection selection = TextSelectHelper.Select(_textArea, type);
				_documentManager.Select(selection);
			}
		}

		private void OnDeleteCharacter(object sender, ExecutedRoutedEventArgs e)
		{
			OnDeleteCharacter();
		}

		private void OnDelete(object sender, ExecutedRoutedEventArgs e)
		{
			OnDeleteCharacter(false);
		}

		private void OnDeleteNextWord(object sender, ExecutedRoutedEventArgs e)
		{
			TextSelectHelper.Select(_textArea, SelectionType.RightByWord);
			_textArea.DocumentManager.EditChar(string.Empty);
			_textArea.DocumentManager.ClearSelection();
		}

		private void OnDeletePreviousWord(object sender, ExecutedRoutedEventArgs e)
		{
			TextSelectHelper.Select(_textArea, SelectionType.LeftByWord);
			_textArea.DocumentManager.EditChar(string.Empty);
			_textArea.DocumentManager.ClearSelection();
		}

		private void OnTabForward(object sender, ExecutedRoutedEventArgs e)
		{
			if (_textArea.IsKeyboardFocused)
			{
				_textArea.DocumentManager.EditChar("\t");
				e.Handled = true;
			}
		}

		private void CanRedo(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _textArea.DocumentManager.UndoRedo.CanRedo;
			e.Handled = true;
		}

		private void OnRedo(object sender, ExecutedRoutedEventArgs e)
		{
			if (_textArea.DocumentManager.UndoRedo.CanRedo)
			{
				_textArea.DocumentManager.UndoRedo.Redo();
			}
			e.Handled = true;
		}

		private void CanUndo(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _textArea.DocumentManager.UndoRedo.CanUndo;
			e.Handled = true;
		}

		private void OnUndo(object sender, ExecutedRoutedEventArgs e)
		{
			if (_textArea.DocumentManager.UndoRedo.CanUndo)
			{
				_textArea.DocumentManager.UndoRedo.Undo();
			}
			e.Handled = true;
		}

		private void OnPastePlainText(object sender, ExecutedRoutedEventArgs e)
		{
			Paste(false);
		}

		private void OnPaste(object sender, ExecutedRoutedEventArgs e)
		{
			Paste(_documentManager.DefaultPasteType == PasteType.RichText);
		}

		private void Paste(bool isRichText)
		{
			System.Windows.Forms.IDataObject dataObject;
			try
			{
				dataObject = System.Windows.Forms.Clipboard.GetDataObject();
			}
			catch (Exception)
			{
				return;
			}
			TextHelperInner.PasteText(_textArea, new DataObjectAdapter(dataObject), isRichText);
			_textArea.DocumentManager.ClearSelection();
		}

		private void OnCut(object sender, ExecutedRoutedEventArgs e)
		{
			System.Windows.Clipboard.SetDataObject(DataObjectHelper.CreateDataObjectFromSelection(_textArea));
			_textArea.DocumentManager.EditChar(_textArea.DocumentManager.Selection.ToSegment(), string.Empty);
			_textArea.DocumentManager.ClearSelection();
		}

		private void OnCopy(object sender, ExecutedRoutedEventArgs e)
		{
			System.Windows.Clipboard.SetDataObject(DataObjectHelper.CreateDataObjectFromSelection(_textArea));
		}

		private void OnEnter(object sender, ExecutedRoutedEventArgs e)
		{
			if (_textArea.IsKeyboardFocused)
			{
				_textArea.DocumentManager.EditChar(Environment.NewLine);
				e.Handled = true;
			}
		}

		private void OnDeleteCharacter(bool isBackspace = true)
		{
			ISelection selection = _textArea.DocumentManager.Selection;
			if (selection.IsEmpty)
			{
				ParagraphNode paragraphNodeByCaretOffset = _textArea.DocumentManager.CharsManager.ParagraphTree.GetParagraphNodeByCaretOffset(_textArea.DocumentManager.CaretInfo.CurrentCaretOffset);
				ParagraphProperty paragraphProperty = _textArea.DocumentManager.PropertyManager.GetParagraphProperty(paragraphNodeByCaretOffset.ParagraphNumber - 1);
				CaretOffset startOffset;
				if (isBackspace)
				{
					startOffset = ((paragraphProperty.MarkerProperty == null || paragraphProperty.MarkerProperty.IsEmpty() || !_textArea.DocumentManager.CaretInfo.CurrentCaretOffset.IsAtParagraphStart) ? CaretNavigationHelper.GetPrevCharacterCaretOffset(_textArea) : _textArea.DocumentManager.CaretInfo.CurrentCaretOffset);
				}
				else
				{
					startOffset = CaretNavigationHelper.GetNextCharacterCaretOffset(_textArea);
				}
				selection = new Selection(startOffset, _textArea.DocumentManager.CaretInfo.CurrentCaretOffset);
			}
			_textArea.DocumentManager.EditChar(selection.ToSegment(), string.Empty);
			_textArea.DocumentManager.ClearSelection();
		}

		private void CanCut(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _textArea.DocumentManager.CurrentFeature.HasFeature(Features.CutInner) && !_textArea.DocumentManager.Selection.IsEmpty;
			e.Handled = true;
		}

		private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _textArea.DocumentManager.CurrentFeature.HasFeature(Features.CopyInner) && !_textArea.DocumentManager.Selection.IsEmpty;
			e.Handled = true;
		}

		private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _textArea.DocumentManager.CurrentFeature.HasFeature(Features.PasteInner);
			e.Handled = true;
		}
	}
}
