using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PPTServiceUltra.Ink.Core.Documents;
using PPTServiceUltra.Ink.Core.Windows.Input;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Core.TextMenu;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions.ExtensionalCharInput;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Layout;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Editing
{
	internal class MouseHandler
	{
		private class CapturedInputChar
		{
			public IInputInteropExtensionalChar CaptureChar { get; }

			public DocumentOffset DocumentOffset { get; }

			public Rect RenderBounds { get; }

			public CapturedInputChar(IInputInteropExtensionalChar captureChar, DocumentOffset documentOffset, Rect renderBounds)
			{
				CaptureChar = captureChar;
				DocumentOffset = documentOffset;
				RenderBounds = renderBounds;
			}
		}

		private readonly TextArea _textArea;

		private readonly InputGestureInfo _inputGesture;

		private bool _isHitSelection;

		private readonly Style _menuItemStyle;

		private readonly Style _contextMenuStyle;

		private readonly Style _separatorStyle;

		private const string MenuItemStyleName = "Style.MenuItem.Default";

		private const string ContextMenuStyleName = "Style.ContextMenu.Default";

		private static readonly Lazy<Cursor> VerticalCursor;

		private static readonly Lazy<Cursor> HorizontalCursor;

		private CapturedInputChar CapturedHoverInputChar { get; set; }

		private Dictionary<int, CapturedInputChar> CapturedInputCharList { get; } = new Dictionary<int, CapturedInputChar>();

		static MouseHandler()
		{
			VerticalCursor = new Lazy<Cursor>(() => new Cursor(Application.GetResourceStream(new Uri("pack://application:,,,/CoreLib.Windows.TextEditorPlus;component/Resources/Cursors/VerticalText.cur", UriKind.RelativeOrAbsolute)).Stream));
			HorizontalCursor = new Lazy<Cursor>(() => new Cursor(Application.GetResourceStream(new Uri("pack://application:,,,/CoreLib.Windows.TextEditorPlus;component/Resources/Cursors/HorizontalText.cur", UriKind.RelativeOrAbsolute)).Stream));
		}

		internal MouseHandler(TextArea textArea)
		{
			_textArea = textArea;
			_inputGesture = new InputGestureInfo();
			DeviceInputSource deviceInputSource = new DeviceInputSource(_textArea);
			deviceInputSource.Attach(TextArea_InputDown, TextArea_InputMove, TextArea_InputUp);
			deviceInputSource.InputHover += TextArea_InputHover;
			_textArea.DocumentManager.CurrentFeature.OnFeatureChanged += TextArea_OnFeatureChanged;
			_textArea.DocumentManager.ArrangingTypeChanged +=  (object sender, ValueChangeEventArgs<ArrangingType> args) =>
			{
				UpdateCursor();
			};
			if (_textArea.DocumentManager.CurrentFeature.HasFeature(Features.DragTextInner))
			{
				_textArea.AllowDrop = true;
				_textArea.DragOver += TextArea_OnDragOver;
				_textArea.DragLeave += TextArea_OnDragLeave;
				_textArea.Drop += TextArea_OnDrop;
			}
			if (_textArea.DocumentManager.CurrentFeature.HasFeature(Features.EditTextInner))
			{
				UpdateCursor();
			}
			_contextMenuStyle = _textArea.TryFindResource("Style.ContextMenu.Default") as Style;
			_menuItemStyle = _textArea.TryFindResource("Style.MenuItem.Default") as Style;
			_separatorStyle = _textArea.TryFindResource(MenuItem.SeparatorStyleKey) as Style;
		}

		private void TextArea_InputDown(object sender, DeviceInputEventArgs e)
		{
			Point point = TextHelperInner.CorrectPosition(e.OriginEventArgs, _textArea);
			DocumentOffset documentOffset = GetDocumentOffset(point);
			ExtensionalChar extensionalChar;
			if (TryGetExtensionalChar(documentOffset, out extensionalChar))
			{
				IInputInteropExtensionalChar inputInteropExtensionalChar = extensionalChar as IInputInteropExtensionalChar;
				if (inputInteropExtensionalChar != null)
				{
					CapturedInputChar value;
					if (CapturedInputCharList.TryGetValue(e.Id, out value))
					{
						CapturedInputCharList.Remove(e.Id);
					}
					Rect runBoundsByDocumentOffset = _textArea.RenderManager.GetRunBoundsByDocumentOffset(documentOffset);
					if (!IsDownPointCloseChar(e, runBoundsByDocumentOffset))
					{
						CapturedInputCharList[e.Id] = new CapturedInputChar(inputInteropExtensionalChar, documentOffset, runBoundsByDocumentOffset);
						CleanContextMenu();
						inputInteropExtensionalChar.PointerDown(BuildTextEditorInputEventArgs(e, CapturedInputCharList[e.Id]));
						SetCurrentCaretOffset(documentOffset, runBoundsByDocumentOffset, point);
						_textArea.Caret.Hide();
						return;
					}
				}
			}
			if (e.Button == DeviceButton.Context)
			{
				UpdateContextMenu();
				e.OriginEventArgs.Handled = true;
				return;
			}
			_inputGesture.RecordDown(point, e.Type);
			if (_inputGesture.ClickCount % 2 == 0)
			{
				HandleDoubleClick(e.OriginEventArgs, extensionalChar, documentOffset);
			}
			else if (_inputGesture.ClickCount % 2 == 1 && !e.OriginEventArgs.Handled)
			{
				HandleSingleClick(e.OriginEventArgs, extensionalChar, documentOffset, point);
			}
		}

		private bool IsDownPointCloseChar(DeviceInputEventArgs e, Rect runBounds)
		{
			Vector vector = e.Position - runBounds.TopLeft;
			if (_textArea.DocumentManager.ArrangingType == ArrangingType.Horizontal)
			{
				if (vector.X <= 1.0)
				{
					return true;
				}
			}
			else if (vector.Y <= 1.0)
			{
				return true;
			}
			return false;
		}

		private void TextArea_InputMove(object sender, DeviceInputEventArgs e)
		{
			CapturedInputChar value;
			if (CapturedInputCharList.TryGetValue(e.Id, out value))
			{
				value.CaptureChar.PointerMove(BuildTextEditorInputEventArgs(e, value));
			}
			else if (_isHitSelection && _textArea.DocumentManager.CurrentFeature.HasFeature(Features.DragTextInner))
			{
				HandleDragText();
			}
			else if (_textArea.DocumentManager.CurrentFeature.HasFeature(Features.SelectInner))
			{
				HandleDragSelect(e.OriginEventArgs);
			}
		}

		private void TextArea_InputUp(object sender, DeviceInputEventArgs e)
		{
			CapturedInputChar value;
			if (CapturedInputCharList.TryGetValue(e.Id, out value))
			{
				value.CaptureChar.PointerUp(BuildTextEditorInputEventArgs(e, value));
				CapturedInputCharList.Remove(e.Id);
			}
			else if (e.Button != DeviceButton.Context && !e.OriginEventArgs.Handled && _textArea.IsMouseCaptured)
			{
				if (_isHitSelection)
				{
					Point controlPosition = TextHelperInner.CorrectPosition(e.OriginEventArgs, _textArea);
					CaretOffset caretOffsetByPosition = _textArea.RenderManager.GetCaretOffsetByPosition(controlPosition);
					_textArea.DocumentManager.ClearSelection();
					_textArea.DocumentManager.CaretInfo.CurrentCaretOffset = caretOffsetByPosition;
					_textArea.Caret.Show();
				}
				_isHitSelection = false;
				_textArea.ReleaseMouseCapture();
			}
		}

		private void TextArea_InputHover(object sender, DeviceInputEventArgs e)
		{
			if (!_textArea.IsMouseCaptured)
			{
				if (HandleHoverExtensionalChar(e)?.IsClickSelectionEnable ?? false)
				{
					_textArea.Cursor = Cursors.Hand;
				}
				else
				{
					UpdateCursor();
				}
			}
			if (_textArea.IsMouseCaptured && !e.OriginEventArgs.Handled)
			{
				e.OriginEventArgs.Handled = true;
			}
		}

		private ExtensionalChar HandleHoverExtensionalChar(DeviceInputEventArgs e)
		{
			Point correctPosition = TextHelperInner.CorrectPosition(e.OriginEventArgs, _textArea);
			ExtensionalChar extensionalChar = null;
			if (CapturedHoverInputChar != null)
			{
				if (CapturedHoverInputChar.RenderBounds.Contains(e.Position))
				{
					extensionalChar = (ExtensionalChar)CapturedHoverInputChar.CaptureChar;
				}
				else
				{
					CapturedHoverInputChar.CaptureChar.PointerExited(new TextEditorPointerExitedEventArgs(CapturedHoverInputChar.CaptureChar, e, _textArea));
					CapturedHoverInputChar = null;
				}
			}
			DocumentOffset documentOffset = GetDocumentOffset(correctPosition);
			if (extensionalChar == null)
			{
				TryGetExtensionalChar(documentOffset, out extensionalChar);
			}
			if (extensionalChar != null)
			{
				IInputInteropExtensionalChar inputInteropExtensionalChar = extensionalChar as IInputInteropExtensionalChar;
				if (inputInteropExtensionalChar != null)
				{
					TextArea textArea = _textArea;
					IInputInteropExtensionalChar extensionalChar2 = inputInteropExtensionalChar;
					TextEditorInputEventArgs args = new TextEditorInputEventArgs(e, textArea, documentOffset, null, extensionalChar2);
					if (CapturedHoverInputChar == null)
					{
						inputInteropExtensionalChar.PointerEntered(args);
						Rect runBoundsByDocumentOffset = _textArea.RenderManager.GetRunBoundsByDocumentOffset(documentOffset);
						CapturedHoverInputChar = new CapturedInputChar(inputInteropExtensionalChar, documentOffset, runBoundsByDocumentOffset);
					}
					inputInteropExtensionalChar.PointerHover(args);
				}
			}
			return extensionalChar;
		}

		private void TextArea_OnFeatureChanged()
		{
			if (_textArea.DocumentManager.CurrentFeature.HasFeature(Features.EditTextInner))
			{
				UpdateCursor();
			}
			else
			{
				_textArea.ClearValue(FrameworkElement.CursorProperty);
			}
		}

		private DocumentOffset GetDocumentOffset(Point correctPosition)
		{
			return _textArea.RenderManager.GetDocumentOffsetByPositionFromRunBounds(correctPosition);
		}

		private TextEditorInputEventArgs BuildTextEditorInputEventArgs(DeviceInputEventArgs e, CapturedInputChar capturedInputChar)
		{
			return new TextEditorInputEventArgs(e, _textArea, capturedInputChar.DocumentOffset, capturedInputChar.RenderBounds, capturedInputChar.CaptureChar);
		}

		private void SetCurrentCaretOffset(DocumentOffset documentOffset, Rect runBounds, Point position)
		{
			bool flag = true;
			if (_textArea.DocumentManager.ArrangingType == ArrangingType.Horizontal)
			{
				flag = !(position.X - runBounds.Left > runBounds.Width / 2.0);
			}
			else if (position.Y - runBounds.Top > runBounds.Height / 2.0)
			{
				flag = false;
			}
			CaretOffset currentCaretOffset = ((!flag) ? new CaretOffset
			{
				Offset = documentOffset
			} : new CaretOffset
			{
				Offset = (int)documentOffset - 1
			});
			SetCurrentCaretOffset(currentCaretOffset);
		}

		private void SetCurrentCaretOffset(CaretOffset caretOffset)
		{
			if (caretOffset != CaretOffset.Invalid)
			{
				_textArea.DocumentManager.CaretInfo.CurrentCaretOffset = caretOffset;
			}
		}

		private void SelectChar(DocumentOffset documentOffset)
		{
			_textArea.DocumentManager.Selection = new Selection(new CaretOffset
			{
				Offset = (int)documentOffset - 1
			}, new CaretOffset
			{
				Offset = documentOffset
			});
		}

		private bool TryGetExtensionalChar(DocumentOffset documentOffset, out ExtensionalChar extensionalChar)
		{
			extensionalChar = null;
			if (documentOffset == DocumentOffset.DefaultDocumentOffset)
			{
				return false;
			}
			ICharObject charObject = _textArea.DocumentManager.CharsManager.Chars[documentOffset];
			if (charObject is ExtensionalChar)
			{
				extensionalChar = (ExtensionalChar)charObject;
				return true;
			}
			return false;
		}

		private void CleanContextMenu()
		{
			_textArea.ContextMenu = null;
		}

		private void UpdateContextMenu()
		{
			TextContextMenuProvider customTextContextMenu = TextContextMenuProvider.GetCustomTextContextMenu(_textArea);
			ContextMenu contextMenu;
			if (customTextContextMenu != null)
			{
				contextMenu = customTextContextMenu.BuildContextMenu(_textArea);
			}
			else
			{
				contextMenu = new ContextMenu
				{
					Style = _contextMenuStyle
				};
				AddNormalMenuItem(contextMenu);
				List<MenuItem> hyperlinkMenuItem = GetHyperlinkMenuItem();
				if (hyperlinkMenuItem.Count + _textArea.ExtentionContextMenuItems.Count > 0)
				{
					contextMenu.Items.Add(new Separator
					{
						Style = _separatorStyle
					});
					foreach (MenuItem extentionContextMenuItem in _textArea.ExtentionContextMenuItems)
					{
						contextMenu.Items.Add(extentionContextMenuItem.Clone());
					}
					foreach (MenuItem item in hyperlinkMenuItem)
					{
						contextMenu.Items.Add(item);
					}
				}
			}
			if (contextMenu.Items.Count > 0)
			{
				_textArea.ContextMenu = contextMenu;
			}
		}

		private List<MenuItem> GetHyperlinkMenuItem()
		{
			TextArea textArea = _textArea;
			if (!textArea.DocumentManager.CurrentFeature.HasFeature(Features.ChangeHyperlinkInner))
			{
				return new List<MenuItem>(0);
			}
			Style menuItemStyle = _menuItemStyle;
			return MenuItemExtension.GetHyperlinkMenuItem(textArea, menuItemStyle);
		}

		private void AddNormalMenuItem(ContextMenu textContextMenu)
		{
			Feature currentFeature = _textArea.DocumentManager.CurrentFeature;
			MenuItemExtension.AddNormalMenuItem(textContextMenu, currentFeature, _menuItemStyle);
		}

		private void TextArea_OnDrop(object sender, DragEventArgs e)
		{
			if (!_textArea.DocumentManager.CurrentFeature.HasFeature(Features.DragTextInner))
			{
				return;
			}
			e.Handled = true;
			_textArea.Caret.SetCaretType(CaretType.Normal);
			if (!TextHelperInner.IsEditorSupport(e))
			{
				_textArea.OnPasteNotSupported();
			}
			else
			{
				if (e.Data.GetDataPresent(DataFormats.FileDrop))
				{
					return;
				}
				Point point = TextHelperInner.CorrectPosition(e, _textArea);
				if (TextHelperInner.IsHittingSelection(_textArea, point))
				{
					_textArea.DocumentManager.ClearSelection();
					_textArea.OnDropTextCompleted();
					return;
				}
				using (IUndoRedoTransaction undoRedoTransaction = _textArea.DocumentManager.UndoRedo.BeginTransaction())
				{
					TextEditor textEditor = e.Data.GetData("source") as TextEditor;
					if (textEditor != null)
					{
						if (!textEditor.TextArea.Equals(_textArea))
						{
							textEditor.EditTextWithUndoRedo(textEditor.Selection.ToSegment(), string.Empty);
							CaretOffset currentCaretOffset = _textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
							TextHelperInner.PasteText(_textArea, new DataObjectAdapter(e.Data), true);
							_textArea.DocumentManager.Selection = new Selection(currentCaretOffset, _textArea.DocumentManager.CaretInfo.CurrentCaretOffset);
						}
						else
						{
							ISelection selection = _textArea.DocumentManager.Selection;
							CaretOffset currentCaretOffset2 = _textArea.DocumentManager.CaretInfo.CurrentCaretOffset;
							_textArea.DocumentManager.ClearSelection();
							if (selection.BehindOffset.Offset < currentCaretOffset2.Offset)
							{
								_textArea.DocumentManager.EditChar(selection.ToSegment(), string.Empty);
								int num = currentCaretOffset2.Offset - selection.Length;
								_textArea.DocumentManager.CaretInfo.CurrentCaretOffset = new CaretOffset
								{
									Offset = num
								};
								int num2 = TextHelperInner.PasteText(_textArea, new DataObjectAdapter(e.Data), true);
								_textArea.DocumentManager.Selection = new Selection(new CaretOffset
								{
									Offset = num
								}, new CaretOffset
								{
									Offset = num + num2
								});
							}
							else if (selection.FrontOffset.Offset > currentCaretOffset2.Offset)
							{
								_textArea.DocumentManager.EditChar(selection.ToSegment(), string.Empty);
								_textArea.DocumentManager.CaretInfo.CurrentCaretOffset = currentCaretOffset2;
								int num3 = TextHelperInner.PasteText(_textArea, new DataObjectAdapter(e.Data), true);
								_textArea.DocumentManager.Selection = new Selection(currentCaretOffset2, new CaretOffset
								{
									Offset = currentCaretOffset2.Offset + num3
								});
							}
						}
					}
					else
					{
						TextHelperInner.PasteText(_textArea, new DataObjectAdapter(e.Data), true);
					}
					undoRedoTransaction.Commit();
				}
				_textArea.OnDropTextCompleted();
			}
		}

		private void TextArea_OnDragLeave(object sender, DragEventArgs e)
		{
			_textArea.Caret.SetCaretType(CaretType.Transparent);
			_textArea.Caret.Show();
		}

		private void TextArea_OnDragOver(object sender, DragEventArgs e)
		{
			_textArea.Caret.SetCaretType(CaretType.DragOver);
			Point controlPosition = TextHelperInner.CorrectPosition(e, _textArea);
			CaretOffset caretOffsetByPosition = _textArea.RenderManager.GetCaretOffsetByPosition(controlPosition);
			_textArea.DocumentManager.CaretInfo.CurrentCaretOffset = caretOffsetByPosition;
		}

		private void UpdateCursor()
		{
			_textArea.Cursor = ((_textArea.DocumentManager.ArrangingType == ArrangingType.Horizontal) ? HorizontalCursor.Value : VerticalCursor.Value);
		}

		private void HandleSingleClick(InputEventArgs e, ExtensionalChar extensionalChar, DocumentOffset documentOffset, Point correctPosition)
		{
			CaretOffset caretOffsetByPosition = _textArea.RenderManager.GetCaretOffsetByPosition(correctPosition);
			_isHitSelection = TextHelperInner.IsHittingSelection(_textArea, caretOffsetByPosition);
			if (_textArea.DocumentManager.CharsManager.CharCount == 0)
			{
				_isHitSelection = false;
			}
			if (!_isHitSelection)
			{
				if (extensionalChar?.IsClickSelectionEnable ?? false)
				{
					_textArea.DocumentManager.Selection = new Selection(caretOffsetByPosition, new CaretOffset
					{
						Offset = documentOffset
					});
				}
				else
				{
					_textArea.DocumentManager.ClearSelection();
					SetCurrentCaretOffset(caretOffsetByPosition);
				}
			}
			_textArea.Focus();
			Mouse.Capture(_textArea, CaptureMode.SubTree);
			_textArea.Caret.SetCaretType(CaretType.Normal);
			_textArea.Caret.Show();
			e.Handled = true;
		}

		private void HandleDoubleClick(InputEventArgs e, ExtensionalChar extensionalChar, DocumentOffset documentOffset)
		{
			if (extensionalChar?.IsClickSelectionEnable ?? false)
			{
				SelectChar(documentOffset);
				_textArea.OnDoubleClickExtensionalChar(extensionalChar);
				_isHitSelection = false;
				e.Handled = true;
			}
			else if (_textArea.DocumentManager.CurrentFeature.HasFeature(Features.SelectInner))
			{
				_textArea.DocumentManager.SelectAll();
				e.Handled = true;
			}
		}

		private void HandleDragText()
		{
			_textArea.OnDragTextStarting();
			DragDrop.DoDragDrop(_textArea, DataObjectHelper.CreateDataObjectFromSelection(_textArea), DragDropEffects.Move);
			_textArea.OnDragTextCompleted();
			_isHitSelection = false;
		}

		private void HandleDragSelect(RoutedEventArgs e)
		{
			if (_inputGesture.ClickCount % 2 != 0)
			{
				CaretOffset startOffset = (_textArea.DocumentManager.Selection.IsEmpty ? _textArea.DocumentManager.CaretInfo.CurrentCaretOffset : _textArea.DocumentManager.Selection.StartOffset);
				Point controlPosition = TextHelperInner.CorrectPosition(e, _textArea);
				CaretOffset caretOffsetByPosition = _textArea.RenderManager.GetCaretOffsetByPosition(controlPosition);
				CaretOffset caretOffsetByPosition2 = _textArea.RenderManager.GetCaretOffsetByPosition(controlPosition);
				_textArea.DocumentManager.Selection = new Selection(startOffset, caretOffsetByPosition2);
				_textArea.DocumentManager.CaretInfo.CurrentCaretOffset = caretOffsetByPosition;
			}
		}
	}
}
