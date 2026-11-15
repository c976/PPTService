using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.Input;
using PPTService.Ink.Core.Windows.TextEditorPlus.Core;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Editing
{
	internal class CommandHandler : ILazyInitialize
	{
		private readonly DocumentManager _documentManager;

		internal TextArea TextArea { get; }

		internal CommandHandler(TextArea textArea)
		{
			TextArea = textArea;
			_documentManager = textArea.DocumentManager;
			SetCommands();
		}

		public void InitializeUntilEditable()
		{
			SetInputKeys();
		}

		public void InitializeUntilNotEmpty()
		{
		}

		private void SetCommands()
		{
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.IncreaseFontSize, OnIncreaseFontSize));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.DecreaseFontSize, OnDecreaseFontSize));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.SetFontSizeCommand, OnSetFontSize));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.SetFontFamilyCommand, OnSetFontFamily));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.SetForegroundCommand, OnSetForeground));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.TextEffectsCommand, OnSetTextEffects));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.ToggleUnderline, OnToggleUnderline));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.ToggleBold, OnToggleBold));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.ToggleItalic, OnToggleItalic));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.ToggleSubscript, OnSetFontSubscript));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.ToggleSuperscript, OnSetFontSuperscript));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ApplyFormatPainterCommand, OnApplyFormatPainter));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.ResetCommand, OnReset));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.MarkerCommand, OnSetMarker));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.SetLineSpaceCommand, OnSetLineSpace));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.DecreaseIndentation, OnDecreaseIndentation));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.IncreaseIndentation, OnIncreaseIndentation));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.AlignLeft, OnAlignLeft));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.AlignCenter, OnAlignCenter));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.AlignRight, OnAlignRight));
			TextArea.CommandBindings.Add(new CommandBinding(EditingCommands.AlignJustify, OnAlignJustify));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.AlignTopCommand, OnAlignTop));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.AlignMiddleCommand, OnAlignVerticalCenter));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.AlignBottomCommand, OnAlignBottom));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.SetParagraphSpaceBeforeCommand, OnSetParagraphSpaceBefore));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.SetParagraphSpaceAfterCommand, OnSetParagraphSpaceAfter));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.SetDirectionCommand, OnSetDirection));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.SetTextArrangingTypeCommand, OnSetTextArrangingType));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.AddHyperlinkCommand, OnAddHyperlink, CanAddHyperlink));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.RaiseHyperlinkCommand, OnRaiseHyperlink, CanRaiseHyperlink));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.EditHyperlinkCommand, OnEditHyperlink, CanEditHyperlink));
			TextArea.CommandBindings.Add(new CommandBinding(SetFormatCommands.RemoveHyperlinkCommand, OnRemoveHyperlink, CanRemoveHyperlink));
		}

		private void SetInputKeys()
		{
			TextArea.InputBindings.Add(new InputBinding(EditingCommands.ToggleBold, new KeyGesture(Key.B, ModifierKeys.Control)));
			TextArea.InputBindings.Add(new InputBinding(EditingCommands.ToggleItalic, new KeyGesture(Key.I, ModifierKeys.Control)));
			TextArea.InputBindings.Add(new InputBinding(EditingCommands.ToggleUnderline, new KeyGesture(Key.U, ModifierKeys.Control)));
			TextArea.InputBindings.Add(new InputBinding(EditingCommands.ToggleSubscript, new KeyGesture(Key.OemPlus, ModifierKeys.Control)));
			TextArea.InputBindings.Add(new InputBinding(EditingCommands.ToggleSuperscript, new KeyGesture(Key.OemPlus, ModifierKeys.Control | ModifierKeys.Shift)));
			TextArea.InputBindings.Add(new InputBinding(EditingCommands.AlignLeft, new KeyGesture(Key.L, ModifierKeys.Control)));
			TextArea.InputBindings.Add(new InputBinding(EditingCommands.AlignCenter, new KeyGesture(Key.E, ModifierKeys.Control)));
			TextArea.InputBindings.Add(new InputBinding(EditingCommands.AlignRight, new KeyGesture(Key.R, ModifierKeys.Control)));
			TextArea.InputBindings.Add(new InputBinding(SetFormatCommands.AlignTopCommand, new KeyGesture(Key.T, ModifierKeys.Alt)));
			TextArea.InputBindings.Add(new InputBinding(SetFormatCommands.AlignMiddleCommand, new KeyGesture(Key.M, ModifierKeys.Alt)));
			TextArea.InputBindings.Add(new InputBinding(SetFormatCommands.AlignBottomCommand, new KeyGesture(Key.B, ModifierKeys.Alt)));
			TextArea.InputBindings.Add(new InputBinding(new DelegateCommand(() =>
			{
				TextArea.OnHyperlinkAdded();
			}, () => HyperlinkHelper.CanAddHyperlink(TextArea)), new KeyGesture(Key.K, ModifierKeys.Control)));
		}

		private void OnIncreaseFontSize(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.IncreaseFontSize();
		}

		private void OnDecreaseFontSize(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.DecreaseFontSize();
		}

		private void OnSetFontSize(object sender, ExecutedRoutedEventArgs e)
		{
			double fontSize = Convert.ToDouble(e.Parameter);
			_documentManager.SetFontSize(fontSize);
		}

		private void OnSetFontFamily(object sender, ExecutedRoutedEventArgs e)
		{
			FontFamily fontFamily = (FontFamily)e.Parameter;
			_documentManager.SetFontFamily(fontFamily);
		}

		private void OnSetForeground(object sender, ExecutedRoutedEventArgs e)
		{
			Brush foreground = (Brush)e.Parameter;
			_documentManager.SetForeground(foreground);
		}

		private void OnSetTextEffects(object sender, ExecutedRoutedEventArgs e)
		{
			EffectArgsCollection effectArgs = (EffectArgsCollection)e.Parameter;
			_documentManager.ToggleTextEffects(effectArgs);
		}

		private void OnToggleUnderline(object sender, ExecutedRoutedEventArgs e)
		{
			object parameter = e.Parameter;
			if (parameter != null)
			{
				TextDecorationCollection textDecorationCollection = parameter as TextDecorationCollection;
				if (textDecorationCollection == null)
				{
					throw new ArgumentException("期望传入的开启或关闭下划线的参数不是 TextDecorationCollection 实际类型是" + e.Parameter.GetType());
				}
				_documentManager.SetUnderline(textDecorationCollection);
			}
			else
			{
				_documentManager.ToggleUnderline();
			}
		}

		private void OnToggleBold(object sender, ExecutedRoutedEventArgs e)
		{
			FontWeight fontWeight = FontWeights.Bold;
			if (e.Parameter == null)
			{
				ISegment propertySetSegment = _documentManager.GetPropertySetSegment();
				if (_documentManager.GetRunPropertiesBySegment(propertySetSegment).All((RunProperty x) => x.FontWeight.Equals(FontWeights.Bold)))
				{
					fontWeight = FontWeights.Normal;
				}
			}
			else
			{
				fontWeight = (FontWeight)e.Parameter;
			}
			_documentManager.SetFontWeight(fontWeight);
		}

		private void OnToggleItalic(object sender, ExecutedRoutedEventArgs e)
		{
			FontStyle fontStyle = FontStyles.Italic;
			if (e.Parameter == null)
			{
				ISegment propertySetSegment = _documentManager.GetPropertySetSegment();
				if (_documentManager.GetRunPropertiesBySegment(propertySetSegment).All((RunProperty x) => x.FontStyle.Equals(FontStyles.Italic)))
				{
					fontStyle = FontStyles.Normal;
				}
			}
			else
			{
				fontStyle = (FontStyle)e.Parameter;
			}
			_documentManager.SetFontStyle(fontStyle);
		}

		private void OnSetFontSubscript(object sender, ExecutedRoutedEventArgs e)
		{
			FontVariants fontVariants = FontVariants.Subscript;
			if (e.Parameter == null)
			{
				ISegment propertySetSegment = _documentManager.GetPropertySetSegment();
				if (_documentManager.GetRunPropertiesBySegment(propertySetSegment).All((RunProperty x) => Converter.AreEquals(x.FontVariants, FontVariants.Subscript)))
				{
					fontVariants = FontVariants.Normal;
				}
			}
			else
			{
				fontVariants = (FontVariants)e.Parameter;
			}
			_documentManager.SetFontVariants(fontVariants);
		}

		private void OnSetFontSuperscript(object sender, ExecutedRoutedEventArgs e)
		{
			FontVariants fontVariants = FontVariants.Superscript;
			if (e.Parameter == null)
			{
				ISegment propertySetSegment = _documentManager.GetPropertySetSegment();
				if (_documentManager.GetRunPropertiesBySegment(propertySetSegment).All((RunProperty x) => Converter.AreEquals(x.FontVariants, FontVariants.Superscript)))
				{
					fontVariants = FontVariants.Normal;
				}
			}
			else
			{
				fontVariants = (FontVariants)e.Parameter;
			}
			_documentManager.SetFontVariants(fontVariants);
		}

		private void OnApplyFormatPainter(object sender, ExecutedRoutedEventArgs e)
		{
			TextEditorProperty textEditorProperty = e.Parameter as TextEditorProperty;
			if (textEditorProperty != null)
			{
				_documentManager.ApplyFormatPainter(textEditorProperty);
			}
		}

		private void OnReset(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.Reset();
		}

		private void OnSetMarker(object sender, ExecutedRoutedEventArgs e)
		{
			MarkerStyle marker = (MarkerStyle)e.Parameter;
			_documentManager.SetMarker(marker);
		}

		private void OnSetLineSpace(object sender, ExecutedRoutedEventArgs e)
		{
			double result;
			if (double.TryParse(e.Parameter.ToString(), out result))
			{
				_documentManager.SetLineSpace(result);
			}
		}

		private void OnDecreaseIndentation(object sender, ExecutedRoutedEventArgs e)
		{
			if (!TextHelperInner.IsMinIndent(TextArea))
			{
				_documentManager.DecreaseIndentation();
			}
		}

		private void OnIncreaseIndentation(object sender, ExecutedRoutedEventArgs e)
		{
			if (TextHelperInner.IsWidthEnoughToIndent(TextArea) && !TextHelperInner.IsMaxIndent(TextArea))
			{
				_documentManager.IncreaseIndentation();
			}
		}

		private void OnAlignLeft(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.AlignLeft();
		}

		private void OnAlignCenter(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.AlignCenter();
		}

		private void OnAlignRight(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.AlignRight();
		}

		private void OnAlignJustify(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.AlignJustify();
		}

		private void OnAlignTop(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.AlignTop();
		}

		private void OnAlignVerticalCenter(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.AlignVerticalCenter();
		}

		private void OnAlignBottom(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.AlignBottom();
		}

		private void OnSetParagraphSpaceBefore(object sender, ExecutedRoutedEventArgs e)
		{
			double paragraphSpaceBefore = Convert.ToDouble(e.Parameter);
			_documentManager.SetParagraphSpaceBefore(paragraphSpaceBefore);
		}

		private void OnSetParagraphSpaceAfter(object sender, ExecutedRoutedEventArgs e)
		{
			double paragraphSpaceAfter = Convert.ToDouble(e.Parameter);
			_documentManager.SetParagraphSpaceAfter(paragraphSpaceAfter);
		}

		private void OnSetDirection(object sender, ExecutedRoutedEventArgs e)
		{
			FlowDirection result;
			if (Enum.TryParse<FlowDirection>(e.Parameter.ToString(), out result))
			{
				_documentManager.SetDirection(result);
			}
		}

		private void OnSetTextArrangingType(object sender, ExecutedRoutedEventArgs e)
		{
			ArrangingType textArrangingType = (ArrangingType)e.Parameter;
			_documentManager.SetTextArrangingType(textArrangingType);
		}

		private void OnAddHyperlink(object sender, ExecutedRoutedEventArgs e)
		{
			string text = e.Parameter?.ToString();
			if (text != null)
			{
				_documentManager.ChangeHyperlink(text);
			}
		}

		private void OnEditHyperlink(object sender, ExecutedRoutedEventArgs e)
		{
			string text = e.Parameter?.ToString();
			if (text != null)
			{
				_documentManager.ChangeHyperlink(text);
			}
		}

		private void OnRemoveHyperlink(object sender, ExecutedRoutedEventArgs e)
		{
			_documentManager.ChangeHyperlink(null);
		}

		private void OnRaiseHyperlink(object sender, ExecutedRoutedEventArgs e)
		{
			List<int> segment = HyperlinkHelper.GetSelectionHyperlinkIndexes(TextArea).First();
			TextArea.OnRaiseHyperlinkByDocumentOffsets(segment);
		}

		private void CanAddHyperlink(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = HyperlinkHelper.CanAddHyperlink(TextArea);
		}

		private void CanEditHyperlink(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = HyperlinkHelper.CanEditHyperlink(TextArea);
		}

		private void CanRemoveHyperlink(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = HyperlinkHelper.CanRemoveHyperlink(TextArea);
		}

		private void CanRaiseHyperlink(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = HyperlinkHelper.CanRaiseHyperlink(TextArea);
		}
	}
}
