using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Documents;
using PPTServiceUltra.Ink.Core.Linq;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;
using Linq;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class DocumentManager : MarshalByRefObject, IDocumentOperator
	{
		private ISelection _selection = new Selection(CaretOffset.Default, CaretOffset.Default);

		private VerticalAlignment _textVerticalAlignment;

		private SizeToContent _sizeToContent;

		private ArrangingType _arrangingType;

		private IUndoRedoProvider _undoRedo;

		private double _editorWidth;

		private double _editorHeight;

		internal CharsManager CharsManager { get; }

		internal PropertyManager PropertyManager { get; }

		internal string AcceptingTypeRegexString { get; set; } = string.Empty;

		internal IndentType DefaultIndentType { get; set; }

		internal ISelection Selection
		{
			get
			{
				return _selection;
			}
			set
			{
				if (!object.Equals(_selection, value) && (!_selection.IsEmpty || !value.IsEmpty) && CurrentFeature.HasFeature(Features.SelectInner))
				{
					ISelection selection = _selection;
					if (value == null)
					{
						throw new ArgumentNullException("value");
					}
					_selection = value;
					SelectionChanged?.Invoke(this, new ValueChangeEventArgs<ISelection>(selection, _selection));
				}
			}
		}

		internal Feature CurrentFeature { get; set; } = Features.FullFeature;

		internal double EditorWidth
		{
			get
			{
				return _editorWidth;
			}
			set
			{
				double editorWidth = _editorWidth;
				_editorWidth = value;
				if (Math.Abs(editorWidth - _editorWidth) > 1E-05)
				{
					EditorSizeChanged?.Invoke(this, new ValueChangeEventArgs<Size>(new Size(editorWidth, EditorHeight), new Size(_editorWidth, EditorHeight)));
				}
			}
		}

		internal double EditorHeight
		{
			get
			{
				return _editorHeight;
			}
			set
			{
				double editorHeight = _editorHeight;
				_editorHeight = value;
				if (Math.Abs(editorHeight - _editorHeight) > 1E-05)
				{
					EditorSizeChanged?.Invoke(this, new ValueChangeEventArgs<Size>(new Size(EditorWidth, editorHeight), new Size(EditorWidth, _editorHeight)));
				}
			}
		}

		internal int MaxLength { get; set; } = int.MaxValue;

		internal VerticalAlignment TextVerticalAlignment
		{
			get
			{
				return _textVerticalAlignment;
			}
			set
			{
				if (_textVerticalAlignment != value)
				{
					VerticalAlignment textVerticalAlignment = _textVerticalAlignment;
					_textVerticalAlignment = value;
					TextVerticalAlignmentChanged?.Invoke(this, new ValueChangeEventArgs<VerticalAlignment>(textVerticalAlignment, _textVerticalAlignment));
				}
			}
		}

		internal ArrangingType ArrangingType
		{
			get
			{
				return _arrangingType;
			}
			set
			{
				if (_arrangingType != value)
				{
					ArrangingType arrangingType = _arrangingType;
					_arrangingType = value;
					ArrangingTypeChanged?.Invoke(this, new ValueChangeEventArgs<ArrangingType>(arrangingType, _arrangingType));
				}
			}
		}

		internal bool IsDocumentBuilding { get; set; }

		internal bool IsEditable { get; set; } = true;

		internal CaretInfo CaretInfo { get; set; } = new CaretInfo();

		internal bool SelectionContainsEmptyLine { get; set; } = true;

		internal SizeToContent SizeToContent
		{
			get
			{
				return _sizeToContent;
			}
			set
			{
				if (_sizeToContent != value)
				{
					SizeToContent sizeToContent = _sizeToContent;
					_sizeToContent = value;
					SizeToContentChanged?.Invoke(this, new ValueChangeEventArgs<SizeToContent>(sizeToContent, _sizeToContent));
				}
			}
		}

		internal AcceptingInputTypes AcceptingInputType { get; set; } = AcceptingInputTypes.All;

		internal bool IsExtCharEditSupported { get; set; }

		internal bool IsUndoRedoEnabled { get; set; } = true;

		internal PasteType DefaultPasteType { get; set; } = PasteType.RichText;

		internal IUndoRedoProvider UndoRedo
		{
			get
			{
				return _undoRedo ?? TextContext.UndoRedo;
			}
			set
			{
				_undoRedo = value;
			}
		}

		internal double MaxWidth { get; set; } = double.NaN;

		internal double MinWidth { get; set; } = double.NaN;

		internal double MaxHeight { get; set; } = double.NaN;

		internal double MinHeight { get; set; } = double.NaN;

		internal event ValueChangedEventHandler<SizeToContent> SizeToContentChanged;

		internal event ValueChangedEventHandler<ISelection> SelectionChanged;

		internal event EventHandler<CharChangeEventArgs> DocumentChanging;

		internal event EventHandler DocumentSizeChanging;

		internal event EventHandler<CharChangeEventArgs> DocumentChanged;

		internal event EventHandler<StyleChangeEventArgs> StyleChanging;

		internal event EventHandler<StyleChangeEventArgs> StyleChanged;

		internal event ValueChangedEventHandler<ArrangingType> ArrangingTypeChanged;

		internal event ValueChangedEventHandler<VerticalAlignment> TextVerticalAlignmentChanged;

		internal event ValueChangedEventHandler<Size> EditorSizeChanged;

		internal void OnDocumentChanging(CharChangeEventArgs textChangeArgs)
		{
			DocumentChanging?.Invoke(this, textChangeArgs);
		}

		internal void OnDocumentChanged(CharChangeEventArgs textChangeArgs)
		{
			DocumentChanged?.Invoke(this, textChangeArgs);
		}

		internal void OnStyleChanging(StyleChangeEventArgs styleChangeEventArgs)
		{
			StyleChanging?.Invoke(this, styleChangeEventArgs);
		}

		internal void OnStyleChanged(StyleChangeEventArgs styleChangeEventArgs)
		{
			StyleChanged?.Invoke(this, styleChangeEventArgs);
		}

		public DocumentManager(CharsManager charsManager, PropertyManager propertyManager)
		{
			CharsManager = charsManager;
			PropertyManager = propertyManager;
			CharsManager.CharsChanged += CharsChanged;
			DocumentChanged += DocumentManager_OnDocumentChanged;
			CaretInfo.OffsetChanged += CaretInfoOnOffsetChanged;
			new DocumentOperatorUndoRedoService(this);
			StyleChanging += (object sender, StyleChangeEventArgs args) =>
			{
				IsDocumentBuilding = true;
			};
			DocumentChanging += (object sender, CharChangeEventArgs args) =>
			{
				IsDocumentBuilding = true;
			};
		}

		private void DocumentManager_OnDocumentChanged(object sender, CharChangeEventArgs obj)
		{
			CaretInfo.CurrentCaretOffset = CharsManager.GetNewCaretOffset(obj);
		}

		private void CaretInfoOnOffsetChanged(object sender, ValueChangeEventArgs<CaretOffset> e)
		{
			ParagraphNode paragraphNodeByCaretOffset = CharsManager.ParagraphTree.GetParagraphNodeByCaretOffset(e.NewValue);
			PropertyManager.SetCurrentProperty(PropertyManager.GetParagraphProperty(paragraphNodeByCaretOffset.ParagraphNumber - 1));
			if (e.NewValue.IsAtLineStart)
			{
				PropertyManager.SetCurrentProperty(PropertyManager.CurrentParagraphProperty.DefaultRunProperty.Clone());
			}
			else
			{
				PropertyManager.SetCurrentProperty(PropertyManager.GetRunProperty(e.NewValue.Offset).Clone());
			}
		}

		internal void EditChar(string charString)
		{
			List<ICharObject> charsFromText = TextUtilities.GetCharsFromText(charString);
			EditChars(charsFromText);
		}

		internal void EditChar(ISegment segment, string charString)
		{
			List<ICharObject> charsFromText = TextUtilities.GetCharsFromText(charString);
			EditChars(segment, charsFromText);
		}

		internal void EditChar(ICharObject charObject)
		{
			EditChars(new List<ICharObject> { charObject });
		}

		internal void EditChars(List<ICharObject> charObjects)
		{
			ISegment segment;
			if (!Selection.IsEmpty)
			{
				segment = Selection.ToSegment();
			}
			else
			{
				ISegment segment2 = new SimpleSegment(CaretInfo.CurrentCaretOffset.Offset + 1, 0);
				segment = segment2;
			}
			EditChars(segment, charObjects);
		}

		internal void EditChar(ISegment segment, ICharObject charObject)
		{
			EditChars(segment, new List<ICharObject> { charObject });
		}

		public virtual void EditChars(ISegment segment, List<ICharObject> charObjects)
		{
			if (!CanExecute("EditChars"))
			{
				TextTracer.Debug("因为没有开启 EditChars 功能，所以文本不接收输入", "文本输入");
				return;
			}
			List<ICharObject> list = TextHelperInner.PreHandleNewLine(charObjects);
			if (CheckAcceptingChars(list))
			{
				if (!IsExtCharEditSupported && list.OfType<ExtensionalChar>().Any())
				{
					throw new NotSupportedException("edit extensional char must set IsExtCharEditSupported to true");
				}
				UpdateExtensionalCharsSize(list);
				int num = CalcValidInputLength(segment);
				List<ICharObject> list2 = ((num > list.Count) ? list : list.Take(num).ToList());
				CharChangeEventArgs textChangeArgs = new CharChangeEventArgs(segment.StartOffset, CharsManager.GetChars(segment.StartOffset, segment.Length), list2);
				OnDocumentChanging(textChangeArgs);
				CharsManager.Replace(segment, list2);
				PropertyManager.ReplaceWithCurrentProperty(segment, list2.Count);
				OnDocumentChanged(textChangeArgs);
			}
		}

		private void UpdateExtensionalCharsSize(IEnumerable<ICharObject> charObjects)
		{
			if (ArrangingType == ArrangingType.Horizontal)
			{
				return;
			}
			foreach (ICharObject charObject in charObjects)
			{
				ExtensionalChar extensionalChar = charObject as ExtensionalChar;
				if (extensionalChar != null && extensionalChar.IsNeedRotationUnderVertical)
				{
					extensionalChar.Size = new Size(extensionalChar.Size.Height, extensionalChar.Size.Width);
				}
			}
		}

		internal void ReplaceSelectionWithCharAndAllProperty(ISegment segment, List<ICharObject> chars, List<int> newLinePropIndexes, List<int> newRunPropIndexes)
		{
			if (CheckAcceptingChars(chars))
			{
				int num = CalcValidInputLength(segment);
				List<ICharObject> list = ((num > chars.Count) ? chars : chars.Take(num).ToList());
				CharChangeEventArgs textChangeArgs = new CharChangeEventArgs(segment.StartOffset, CharsManager.GetChars(segment.StartOffset, segment.Length), list);
				OnDocumentChanging(textChangeArgs);
				CharsManager.Replace(segment, list);
				if (CharsManager.CharCount != newRunPropIndexes.Count)
				{
					throw new InvalidOperationException("the count of newRunPropIndexes must equal to the length of new Text");
				}
				if (CharsManager.ParagraphTree.ParagraphCount != newLinePropIndexes.Count)
				{
					throw new InvalidOperationException("the count of newLinePropIndexes must equal to the count of new Text's lines");
				}
				PropertyManager.ParagraphProperties = new List<int>(newLinePropIndexes);
				PropertyManager.RunProperties = new List<int>(newRunPropIndexes);
				OnDocumentChanged(textChangeArgs);
			}
		}

		internal void ReplaceSelectionWithRichTextProperty(ISegment segment, RichTextProperty richTextProperty)
		{
			if (!CheckAcceptingChars(richTextProperty.CharObjects))
			{
				return;
			}
			foreach (ICharObject charObject in richTextProperty.CharObjects)
			{
				ExtensionalChar extensionalChar = charObject as ExtensionalChar;
				if (extensionalChar != null && extensionalChar.IsNeedRotationUnderVertical && ArrangingType != ArrangingType.Horizontal)
				{
					extensionalChar.Size = new Size(extensionalChar.Size.Height, extensionalChar.Size.Width);
				}
			}
			int num = CalcValidInputLength(segment);
			List<ICharObject> list = ((num > richTextProperty.CharObjects.Count) ? richTextProperty.CharObjects : richTextProperty.CharObjects.Take(num).ToList());
			CharChangeEventArgs textChangeArgs = new CharChangeEventArgs(segment.StartOffset, CharsManager.GetChars(segment.StartOffset, segment.Length), list);
			OnDocumentChanging(textChangeArgs);
			CharsManager.Replace(segment, list);
			DocumentSizeChanging?.Invoke(this, EventArgs.Empty);
			PropertyManager.ReplaceWithRichTextProperty(segment, richTextProperty);
			OnDocumentChanged(textChangeArgs);
		}

		internal void InsertOperation(IOperation operation)
		{
			if (IsUndoRedoEnabled)
			{
				UndoRedo.Insert(operation);
			}
		}

		internal void SelectAll()
		{
			Selection = new Selection(CaretOffset.Default, new CaretOffset
			{
				Offset = CharsManager.CharCount - 1
			});
			CaretOffset caretOffset = new CaretOffset
			{
				Offset = CharsManager.CharCount - 1
			};
			CaretInfo.CurrentCaretOffset = TextHelperInner.UpdateCaretOffsetIsAtParagraphStart(this, caretOffset);
		}

		internal void ClearSelection()
		{
			TextTracer.Debug("清空文本选择内容", "文本选择");
			Selection = new Selection(CaretOffset.Default, CaretOffset.Default);
		}

		internal List<int> GetSelectionRunIndexes()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < Selection.Length; i++)
			{
				list.Add(Selection.FrontOffset.Offset + i + 1);
			}
			return list;
		}

		internal List<int> GetSelectionParagraphIndexes()
		{
			List<int> list = new List<int>();
			if (Selection.Length == 0)
			{
				return list;
			}
			foreach (ParagraphNode item in CharsManager.ParagraphTree)
			{
				if (item.FrontOffset <= Selection.FrontOffset.Offset + 1 && Selection.FrontOffset.Offset + 1 < item.FrontOffset + item.Length)
				{
					list.Add(item.ParagraphNumber - 1);
				}
			}
			return list;
		}

		internal IEnumerable<int> GetSamePropertyAroundIndexes(int documentOffset, Func<RunProperty, bool> runPropertyEquals)
		{
			List<int> list = new List<int> { documentOffset };
			for (int num = documentOffset - 1; num >= 0; num--)
			{
				RunProperty runProperty = PropertyManager.GetRunProperty(num);
				if (runPropertyEquals(runProperty))
				{
					list.Add(num);
				}
			}
			for (int i = documentOffset + 1; i < CharsManager.CharCount; i++)
			{
				RunProperty runProperty2 = PropertyManager.GetRunProperty(i);
				if (runPropertyEquals(runProperty2))
				{
					list.Add(i);
				}
			}
			return list;
		}

		internal void UpdateExtensionalCharsSize()
		{
			UpdateExtensionalCharsSize(CharsManager.Chars);
		}

		internal IEnumerable<RunProperty> GetRunPropertiesBySegment(ISegment segment, bool ignoreContinuousSameProperty = true)
		{
			RunProperty lastRunProperty = null;
			for (int i = segment.FrontOffset; i < segment.BehindOffset; i++)
			{
				RunProperty runProperty = PropertyManager.GetRunProperty(i);
				if (ignoreContinuousSameProperty)
				{
					if (lastRunProperty == runProperty)
					{
						continue;
					}
					lastRunProperty = runProperty;
				}
				yield return runProperty;
			}
		}

		internal ISegment GetPropertySetSegment()
		{
			if (!IsEditable)
			{
				return new SimpleSegment(0, CharsManager.CharCount);
			}
			if (Selection.IsEmpty)
			{
				return new SimpleSegment(Math.Max(CaretInfo.CurrentCaretOffset.Offset, 0), 0);
			}
			return Selection.ToSegment();
		}

		internal ISegment GetParagraphSelectedSegment()
		{
			if (!IsEditable)
			{
				return new SimpleSegment(0, CharsManager.CharCount);
			}
			if (Selection.IsEmpty)
			{
				ParagraphNode paragraphNodeByCaretOffset = CharsManager.ParagraphTree.GetParagraphNodeByCaretOffset(CaretInfo.CurrentCaretOffset);
				return new SimpleSegment(paragraphNodeByCaretOffset.StartOffset, paragraphNodeByCaretOffset.Length);
			}
			ParagraphNode paragraphNodeByDocumentOffset = CharsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(Selection.FrontOffset.Offset + 1);
			ParagraphNode paragraphNodeByDocumentOffset2 = CharsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(Selection.BehindOffset.Offset + 1);
			return new SimpleSegment(paragraphNodeByDocumentOffset.StartOffset, paragraphNodeByDocumentOffset2.EndOffset - paragraphNodeByDocumentOffset.StartOffset);
		}

		internal void SetRunProperty(Action<RunProperty> action, PropertyType property)
		{
			ISegment propertySetSegment = GetPropertySetSegment();
			SetRunProperty(action, propertySetSegment, property);
		}

		internal void SetRunProperty(Action<RunProperty> action, ISegment segment, PropertyType property)
		{
			OnStyleChanging(new StyleChangeEventArgs(segment, property));
			foreach (int item in TextHelperInner.GetParagraphIndexBySegment(CharsManager, segment, false))
			{
				ParagraphProperty paragraphProperty = PropertyManager.GetParagraphProperty(item).Clone();
				action(paragraphProperty.DefaultRunProperty);
				int paragraphPropertyIndex = PropertyManager.GetParagraphPropertyIndex(paragraphProperty);
				PropertyManager.ParagraphProperties[item] = paragraphPropertyIndex;
			}
			ParagraphProperty paragraphProperty2 = PropertyManager.CurrentParagraphProperty.Clone();
			action(paragraphProperty2.DefaultRunProperty);
			PropertyManager.SetCurrentProperty(paragraphProperty2);
			List<ISegment> sameMogolianSegments = GetSameMogolianSegments(segment);
			sameMogolianSegments.Add(segment);
			ISegment segment2 = TextHelperInner.Union(sameMogolianSegments);
			for (int i = segment2.StartOffset; i < segment2.EndOffset; i++)
			{
				DocumentOffset documentOffset = i;
				RunProperty runProperty = PropertyManager.GetRunProperty(documentOffset);
				RunProperty runProperty2 = runProperty.Clone();
				action(runProperty2);
				ExtensionalChar extensionalChar = CharsManager.GetCharAt(documentOffset) as ExtensionalChar;
				if (extensionalChar != null)
				{
					TextEditorExtManager.StyleAdapter(extensionalChar, new PropertyChangedArgs(property, runProperty, runProperty2), ArrangingType);
					if ((runProperty.Hyperlink == null && runProperty2.Hyperlink != null) || property == PropertyType.EffectsArgs)
					{
						continue;
					}
				}
				PropertyManager.SetRunProperty(documentOffset, runProperty2);
			}
			RunProperty runProperty3 = PropertyManager.CurrentRunProperty.Clone();
			action(runProperty3);
			PropertyManager.SetCurrentProperty(runProperty3);
			OnStyleChanged(new StyleChangeEventArgs(segment, property));
		}

		internal void SetParagraphProperty(Action<ParagraphProperty> action, PropertyType property)
		{
			ISegment paragraphSelectedSegment = GetParagraphSelectedSegment();
			SetParagraphProperty(action, paragraphSelectedSegment, property);
		}

		internal void SetDocument(Action action, PropertyType property)
		{
			SimpleSegment segment = new SimpleSegment(0, CharsManager.CharCount);
			OnStyleChanging(new StyleChangeEventArgs(segment, property));
			action();
			OnStyleChanged(new StyleChangeEventArgs(segment, property));
		}

		internal void ChangeProperty(Action<RunProperty> propAction)
		{
			PropertyManager.RunPropDic.Values.ForEach(propAction);
			PropertyManager.ParagraphPropDic.Values.ForEach((ParagraphProperty x) =>
			{
				propAction(x.DefaultRunProperty);
			});
			propAction(PropertyManager.CurrentRunProperty);
			propAction(PropertyManager.CurrentParagraphProperty.DefaultRunProperty);
			CharsManager.GeometryPool.SetDirty(0, CharsManager.CharCount);
		}

		internal void ChangeProperty(Action<ParagraphProperty> propAction)
		{
			PropertyManager.ParagraphPropDic.Values.ForEach(propAction);
			propAction(PropertyManager.CurrentParagraphProperty);
		}

		internal void ChangePropertyByIndexes(IEnumerable<int> indexes, Action<RunProperty> propAction)
		{
			foreach (int index in indexes)
			{
				DocumentOffset documentOffset = index;
				RunProperty runProperty = PropertyManager.GetRunProperty(documentOffset);
				RunProperty runProperty2 = runProperty.Clone();
				propAction(runProperty2);
				PropertyManager.SetRunProperty(documentOffset, runProperty2);
				CharsManager.GeometryPool.SetDirty(index, 1);
				ExtensionalChar extensionalChar = CharsManager.GetCharAt(index) as ExtensionalChar;
				if (extensionalChar != null)
				{
					TextEditorExtManager.StyleAdapter(extensionalChar, new PropertyChangedArgs(PropertyType.All, runProperty, runProperty2), ArrangingType);
				}
			}
			propAction(PropertyManager.CurrentRunProperty);
			propAction(PropertyManager.CurrentParagraphProperty.DefaultRunProperty);
		}

		internal void ChangePropertyByIndexes(IEnumerable<int> indexes, Action<ParagraphProperty> propAction)
		{
			foreach (int index in indexes)
			{
				ParagraphProperty paragraphProperty = PropertyManager.GetParagraphProperty(index).Clone();
				propAction(paragraphProperty);
				int paragraphPropertyIndex = PropertyManager.GetParagraphPropertyIndex(paragraphProperty);
				PropertyManager.ParagraphProperties[index] = paragraphPropertyIndex;
			}
			propAction(PropertyManager.CurrentParagraphProperty);
		}

		internal List<ISegment> GetSameMogolianSegments(ISegment segment)
		{
			List<ISegment> list = new List<ISegment>();
			if (segment.Length == 0)
			{
				return list;
			}
			TextChar textChar = CharsManager.GetCharAt(segment.FrontOffset) as TextChar;
			if (textChar != null && TextUtilities.IsLigature(textChar.ToText()))
			{
				list.Add(GetSameMogolianSegment(segment.FrontOffset));
			}
			TextChar textChar2 = CharsManager.GetCharAt(segment.BehindOffset - 1) as TextChar;
			if (textChar2 != null && TextUtilities.IsLigature(textChar2.ToText()))
			{
				list.Add(GetSameMogolianSegment(segment.BehindOffset - 1));
			}
			return list;
		}

		internal ISegment GetSameMogolianSegment(int documentOffset)
		{
			List<int> list = new List<int> { documentOffset };
			for (int i = documentOffset + 1; i < CharsManager.CharCount; i++)
			{
				TextChar textChar = CharsManager.GetCharAt(i) as TextChar;
				if (textChar == null || !TextUtilities.IsLigature(textChar.ToText()))
				{
					break;
				}
				list.Add(i);
			}
			for (int num = documentOffset - 1; num >= 0; num--)
			{
				TextChar textChar2 = CharsManager.GetCharAt(num) as TextChar;
				if (textChar2 == null || !TextUtilities.IsLigature(textChar2.ToText()))
				{
					break;
				}
				list.Add(num);
			}
			return new SimpleSegment(list.Min(), list.Count);
		}

		private void CharsChanged(CharChangeEventArgs obj)
		{
			ClearSelection();
		}

		internal void SetParagraphProperty(Action<ParagraphProperty> action, ISegment segment, PropertyType property)
		{
			OnStyleChanging(new StyleChangeEventArgs(segment, property));
			foreach (int item in TextHelperInner.GetParagraphIndexBySegment(CharsManager, segment))
			{
				ParagraphProperty paragraphProperty = PropertyManager.GetParagraphProperty(item).Clone();
				action(paragraphProperty);
				int paragraphPropertyIndex = PropertyManager.GetParagraphPropertyIndex(paragraphProperty);
				PropertyManager.ParagraphProperties[item] = paragraphPropertyIndex;
			}
			ParagraphProperty paragraphProperty2 = PropertyManager.CurrentParagraphProperty.Clone();
			action(paragraphProperty2);
			PropertyManager.SetCurrentProperty(paragraphProperty2);
			OnStyleChanged(new StyleChangeEventArgs(segment, property));
		}

		private int CalcValidInputLength(ISegment segment)
		{
			return MaxLength - (CharsManager.CharCount - segment.Length);
		}

		private bool CheckAcceptingChars(List<ICharObject> charObjects)
		{
			if (string.IsNullOrEmpty(AcceptingTypeRegexString) && AcceptingInputType == AcceptingInputTypes.All)
			{
				return true;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ICharObject charObject in charObjects)
			{
				TextChar textChar = charObject as TextChar;
				if (textChar != null)
				{
					stringBuilder.Append(textChar.ToText());
				}
				else
				{
					stringBuilder.Append('\ufffd');
				}
			}
			string newText = stringBuilder.ToString();
			if (string.IsNullOrEmpty(AcceptingTypeRegexString))
			{
				return TextUtilities.IsValidInputByAcceptingInputType(AcceptingInputType, newText);
			}
			return TextUtilities.IsValidInputByRegexString(AcceptingTypeRegexString, newText);
		}

		private bool CanExecute([CallerMemberName] string methodName = "")
		{
			return CanExecute(new Feature { methodName });
		}

		private bool CanExecute(IFeature feature)
		{
			if (CurrentFeature.HasFeature(feature))
			{
				return true;
			}
			return false;
		}

		public virtual void MoveCaret(CaretOffset newOffset)
		{
			if (CanExecute("MoveCaret"))
			{
				ClearSelection();
				CaretInfo.CurrentCaretOffset = newOffset;
			}
		}

		public virtual void Select(ISelection selection)
		{
			if (CanExecute("Select"))
			{
				Selection = selection;
				CaretInfo.CurrentCaretOffset = selection.EndOffset;
			}
		}

		public virtual void SetFontSize(double fontSize)
		{
			SetFontSize(fontSize, GetPropertySetSegment());
		}

		public virtual bool SetFontSize(double fontSize, ISegment segment)
		{
			if (!CanExecute("SetFontSize"))
			{
				return false;
			}
			SetRunProperty((RunProperty p) =>
			{
				p.FontSize = fontSize;
			}, segment, PropertyType.FontSize);
			return true;
		}

		public virtual void SetFontFamily(FontFamily fontFamily)
		{
			SetFontFamily(fontFamily, GetPropertySetSegment());
		}

		public virtual bool SetFontFamily(FontFamily fontFamily, ISegment segment)
		{
			if (!CanExecute("SetFontFamily"))
			{
				return false;
			}
			SetRunProperty((RunProperty p) =>
			{
				p.FontFamily = fontFamily;
			}, segment, PropertyType.FontFamily);
			return true;
		}

		public virtual void SetForeground(Brush foreground)
		{
			SetForeground(foreground, GetPropertySetSegment());
		}

		public virtual bool SetForeground(Brush foreground, ISegment segment)
		{
			if (!CanExecute("SetForeground"))
			{
				return false;
			}
			SetRunProperty((RunProperty p) =>
			{
				FillEffectArgs fillEffectArgs = p.EffectsArgs.OfType<FillEffectArgs>().OneOrDefault();
				if (fillEffectArgs != null)
				{
					fillEffectArgs.FillBrush = (foreground.IsFrozen ? foreground : foreground.Clone());
				}
				else
				{
					p.Foreground = foreground;
				}
			}, segment, PropertyType.Foreground);
			return true;
		}

		public virtual void IncreaseFontSize()
		{
			if (CanExecute("IncreaseFontSize"))
			{
				SetRunProperty((RunProperty p) =>
				{
					p.FontSize++;
				}, PropertyType.FontSize);
			}
		}

		public virtual void DecreaseFontSize()
		{
			if (CanExecute("DecreaseFontSize"))
			{
				SetRunProperty((RunProperty p) =>
				{
					p.FontSize--;
				}, PropertyType.FontSize);
			}
		}

		public virtual void SetParagraphSpaceBefore(double value)
		{
			if (CanExecute("SetParagraphSpaceBefore"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					p.ParagraphBefore = value;
				}, PropertyType.ParagraphBefore);
			}
		}

		public virtual void SetParagraphSpaceAfter(double value)
		{
			if (CanExecute("SetParagraphSpaceAfter"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					p.ParagraphAfter = value;
				}, PropertyType.ParagraphAfter);
			}
		}

		public virtual void SetDirection(FlowDirection value)
		{
			if (CanExecute("SetDirection"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					p.Direction = value;
				}, PropertyType.Direction);
			}
		}

		public virtual void SetTextArrangingType(ArrangingType arrangingType)
		{
			if (CanExecute("SetTextArrangingType"))
			{
				SetDocument(() =>
				{
					ArrangingType = arrangingType;
				}, PropertyType.ArrangingType);
			}
		}

		public virtual void ToggleTextEffects(EffectArgsCollection effectArgs)
		{
			if (CanExecute(Features.SetTextEffectsInner))
			{
				ISegment propertySetSegment = GetPropertySetSegment();
				List<RunProperty> source = GetRunPropertiesBySegment(propertySetSegment).ToList();
				if (source.Any() && source.All((RunProperty prop) => Converter.AreEquals(prop.EffectsArgs, effectArgs)))
				{
					effectArgs = new EffectArgsCollection();
				}
				if (!source.Any() && Converter.AreEquals(PropertyManager.CurrentRunProperty.EffectsArgs, effectArgs))
				{
					effectArgs = new EffectArgsCollection();
				}
				SetRunProperty((RunProperty p) =>
				{
					p.EffectsArgs = effectArgs;
				}, PropertyType.EffectsArgs);
			}
		}

		[Obsolete("请使用 ToggleTextEffects 代替，这个方法说的是设置特效，实际逻辑是开启或关闭特效")]
		public virtual void SetTextEffects(EffectArgsCollection effectArgs)
		{
			ToggleTextEffects(effectArgs);
		}

		[Obsolete("此方法因为不能知道参数应该传入什么值，请使用 ToggleUnderline() 代替，如需要直接设置特殊装饰请使用 SetUnderline 方法直接代替")]
		public virtual void ToggleUnderline(TextDecorationCollection textDecoration)
		{
			SetUnderline(textDecoration);
		}

		public virtual void ToggleUnderline()
		{
			ISegment propertySetSegment = GetPropertySetSegment();
			ToggleSegmentUnderline(propertySetSegment);
		}

		public virtual void ToggleUnderline(ISelection selection)
		{
			if (!selection.IsEmpty)
			{
				ToggleSegmentUnderline(selection.ToSegment());
			}
		}

		private void ToggleSegmentUnderline(ISegment segment)
		{
			if (CanExecute(Features.ToggleUnderlineInner))
			{
				TextDecoration underline = InnerTextContext.Underline;
				ToggleTextDecoration(segment, underline);
			}
		}

		public virtual void SetUnderline(TextDecorationCollection textDecoration)
		{
			if (CanExecute(Features.ToggleUnderlineInner))
			{
				SetRunProperty((RunProperty p) =>
				{
					p.TextDecoration = textDecoration;
				}, PropertyType.TextDecoration);
			}
		}

		[Obsolete("请使用 ToggleStrikeThrough 代替。本方法只是让你知道删除线应该叫 Strike 而不是 DeleteLine 而已")]
		public void ToggleDeleteLine(ISelection selection = null)
		{
			ToggleStrikethrough(selection);
		}

		public void ToggleStrikethrough(ISelection selection = null)
		{
			if ((!(selection?.IsEmpty)) ?? true)
			{
				ISegment segment = ((selection != null) ? selection.ToSegment() : GetPropertySetSegment());
				ToggleStrikethrough(segment);
			}
		}

		public void ToggleStrikethrough(ISegment segment)
		{
			if (CanExecute(Features.ToggleStrikethrough))
			{
				TextDecoration strikethrough = InnerTextContext.Strikethrough;
				ToggleTextDecoration(segment, strikethrough);
			}
		}

		private void ToggleTextDecoration(ISegment segment, TextDecoration textDecoration)
		{
			IEnumerable<RunProperty> runPropertiesBySegment = GetRunPropertiesBySegment(segment);
			bool shouldRemoveStrikeThrough = runPropertiesBySegment.All((RunProperty property) => Converter.Contains(property.TextDecoration, textDecoration));
			SetRunProperty((RunProperty property) =>
			{
				if (shouldRemoveStrikeThrough)
				{
					if (Converter.Contains(property.TextDecoration, textDecoration))
					{
						property.TextDecoration = new TextDecorationCollection(property.TextDecoration.Where((TextDecoration t) => !Converter.AreEquals(t, textDecoration)));
					}
				}
				else if (!Converter.Contains(property.TextDecoration, textDecoration))
				{
					property.TextDecoration = new TextDecorationCollection(property.TextDecoration.Concat(new TextDecoration[1] { textDecoration }));
				}
			}, segment, PropertyType.TextDecoration);
		}

		[Obsolete("此方法名是开启或关闭文本加粗，实际作用是设置文本加粗，实现和命名不同，请使用 SetFontWeight 替换")]
		public virtual void ToggleBold(FontWeight fontWeight)
		{
			SetFontWeight(fontWeight);
		}

		public void SetFontWeight(FontWeight fontWeight)
		{
			if (CurrentFeature.HasFeature(Features.ToggleBoldInner))
			{
				SetRunProperty((RunProperty p) =>
				{
					p.FontWeight = fontWeight;
				}, PropertyType.FontWeight);
			}
		}

		[Obsolete("此方法名是开启或关闭文本斜体但实际作为是设置字体样式，实现和命名不同，请使用 SetFontStyle 代替")]
		public virtual void ToggleItalic(FontStyle fontStyle)
		{
			SetFontStyle(fontStyle);
		}

		public void SetFontStyle(FontStyle fontStyle)
		{
			if (CurrentFeature.HasFeature(Features.ToggleItalicInner))
			{
				SetRunProperty((RunProperty p) =>
				{
					p.FontStyle = fontStyle;
				}, PropertyType.FontStyle);
			}
		}

		[Obsolete("此方法名是开启或关闭文本的下标，但实际上是设置文本的上标和下标在相同的方法，实现和命名不同，请使用 SetFontVariants 代替")]
		public virtual void ToggleSubscript(FontVariants fontVariants)
		{
			SetFontVariants(fontVariants);
		}

		[Obsolete("此方法名是开启或关闭文本的上标，但实际上是设置文本的上标和下标在相同的方法，实现和命名不同，请使用 SetFontVariants 代替")]
		public virtual void ToggleSuperscript(FontVariants fontVariants)
		{
			SetFontVariants(fontVariants);
		}

		public void SetFontVariants(FontVariants fontVariants)
		{
			if (fontVariants == FontVariants.Superscript)
			{
				if (!CurrentFeature.HasFeature(Features.SetFontSuperscriptInner))
				{
					return;
				}
			}
			else if (fontVariants == FontVariants.Subscript && !CurrentFeature.HasFeature(new Feature { "ToggleSubscript" }))
			{
				return;
			}
			SetRunProperty((RunProperty p) =>
			{
				p.FontVariants = fontVariants;
			}, PropertyType.FontVariants);
		}

		public virtual void ApplyFormatPainter(TextEditorProperty formatTextEditorProperty)
		{
			if (!CanExecute("ApplyFormatPainter"))
			{
				return;
			}
			using (IUndoRedoTransaction undoRedoTransaction = UndoRedo.BeginTransaction())
			{
				SetRunProperty((RunProperty x) =>
				{
					if (x == null)
					{
						throw new ArgumentNullException("x");
					}
					x.SetRunProperty(formatTextEditorProperty.RunProperty);
				}, PropertyType.RunProperty);
				if (formatTextEditorProperty.ParagraphProperty != null)
				{
					SetParagraphProperty((ParagraphProperty x) =>
					{
						if (x == null)
						{
							throw new ArgumentNullException("x");
						}
						x.SetParagraphProperty(formatTextEditorProperty.ParagraphProperty);
					}, PropertyType.ParagraphProperty);
				}
				if (formatTextEditorProperty.TextVerticalAlignment.HasValue || formatTextEditorProperty.ArrangingType.HasValue)
				{
					SetDocument(() =>
					{
						if (formatTextEditorProperty.TextVerticalAlignment.HasValue)
						{
							TextVerticalAlignment = formatTextEditorProperty.TextVerticalAlignment.Value;
						}
						if (formatTextEditorProperty.ArrangingType.HasValue)
						{
							ArrangingType = formatTextEditorProperty.ArrangingType.Value;
						}
					}, PropertyType.DocumentProperty);
				}
				undoRedoTransaction.Commit();
			}
		}

		public virtual void SetMarker(MarkerStyle markerStyle)
		{
			if (CanExecute("SetMarker"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					ParagraphProperty oldProp = p.Clone();
					p.MarkerProperty = MarkerFactory.GetMarkerProperty(p.DefaultRunProperty, markerStyle);
					TextHelperInner.HandlerMarginLeftAndIndent(p, oldProp);
				}, PropertyType.MarkerStyle);
			}
		}

		public virtual void SetLineSpace(double lineSpace)
		{
			if (CanExecute("SetLineSpace"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					p.LineSpacing = lineSpace;
					p.FixedLineSpacing = double.NaN;
				}, PropertyType.LineSpacing);
			}
		}

		public virtual void DecreaseIndentation()
		{
			if (CanExecute("DecreaseIndentation"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					int indentLevel = p.IndentLevel;
					p.IndentLevel--;
					p.MarginLeft -= 24.0 * (double)(indentLevel - p.IndentLevel) * 1.4;
				}, PropertyType.MarginLeft);
			}
		}

		public virtual void IncreaseIndentation()
		{
			if (CanExecute("IncreaseIndentation"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					int indentLevel = p.IndentLevel;
					p.IndentLevel++;
					p.MarginLeft += 24.0 * (double)(p.IndentLevel - indentLevel) * 1.4;
				}, PropertyType.MarginLeft);
			}
		}

		public virtual void AlignLeft()
		{
			if (CanExecute("AlignLeft"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					p.TextAlignment = TextAlignment.Left;
				}, PropertyType.TextAlignment);
			}
		}

		public virtual void AlignCenter()
		{
			if (CanExecute("AlignCenter"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					p.TextAlignment = TextAlignment.Center;
				}, PropertyType.TextAlignment);
			}
		}

		public virtual void AlignRight()
		{
			if (CanExecute("AlignRight"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					p.TextAlignment = TextAlignment.Right;
				}, PropertyType.TextAlignment);
			}
		}

		public virtual void AlignJustify()
		{
			if (CanExecute("AlignJustify"))
			{
				SetParagraphProperty((ParagraphProperty p) =>
				{
					p.TextAlignment = TextAlignment.Justify;
				}, PropertyType.TextAlignment);
			}
		}

		public virtual void AlignTop()
		{
			if (CanExecute("AlignTop"))
			{
				SetDocument(() =>
				{
					TextVerticalAlignment = VerticalAlignment.Top;
				}, PropertyType.TextVerticalAlignment);
			}
		}

		public virtual void AlignVerticalCenter()
		{
			if (CanExecute("AlignVerticalCenter"))
			{
				SetDocument(() =>
				{
					TextVerticalAlignment = VerticalAlignment.Center;
				}, PropertyType.TextVerticalAlignment);
			}
		}

		public virtual void AlignBottom()
		{
			if (CanExecute("AlignBottom"))
			{
				SetDocument(() =>
				{
					TextVerticalAlignment = VerticalAlignment.Bottom;
				}, PropertyType.TextVerticalAlignment);
			}
		}

		public virtual void Reset()
		{
			if (!CanExecute("Reset"))
			{
				return;
			}
			SetRunProperty((RunProperty x) =>
			{
				if (x == null)
				{
					throw new ArgumentNullException("x");
				}
				x.SetRunProperty(PropertyManager.DefaultRunProperty);
			}, PropertyType.RunProperty);
		}

		public virtual void ChangeHyperlink(string hyperlink)
		{
			if (!CanExecute("ChangeHyperlink"))
			{
				return;
			}
			if (Selection.IsEmpty)
			{
				if (hyperlink == null)
				{
					List<int> hyperlinkIndexesByCaretOffset = HyperlinkHelper.GetHyperlinkIndexesByCaretOffset(this, CaretInfo.CurrentCaretOffset);
					if (hyperlinkIndexesByCaretOffset.Count != 0)
					{
						SetRunProperty((RunProperty p) =>
						{
							p.Hyperlink = null;
						}, new SimpleSegment(hyperlinkIndexesByCaretOffset.First(), hyperlinkIndexesByCaretOffset.Count), PropertyType.Hyperlink);
					}
				}
				else
				{
					SetRunProperty((RunProperty p) =>
					{
						p.Hyperlink = hyperlink;
					}, new SimpleSegment(CaretInfo.CurrentCaretOffset.Offset, 1), PropertyType.Hyperlink);
				}
			}
			else
			{
				SetRunProperty((RunProperty p) =>
				{
					p.Hyperlink = hyperlink;
				}, PropertyType.Hyperlink);
			}
		}
	}
}
