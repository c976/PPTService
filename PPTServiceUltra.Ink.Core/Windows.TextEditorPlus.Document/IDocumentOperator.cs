using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	internal interface IDocumentOperator
	{
		void SetFontSize(double fontSize);

		void SetFontFamily(FontFamily fontFamily);

		void SetForeground(Brush foreground);

		void IncreaseFontSize();

		void DecreaseFontSize();

		void SetParagraphSpaceBefore(double value);

		void SetParagraphSpaceAfter(double value);

		void SetTextArrangingType(ArrangingType arrangingType);

		[Obsolete("请使用 ToggleTextEffects 代替，这个方法说的是设置特效，实际逻辑是开启或关闭特效")]
		void SetTextEffects(EffectArgsCollection effectArgs);

		void ToggleTextEffects(EffectArgsCollection effectArgs);

		[Obsolete("此方法因为不能知道参数应该传入什么值，请使用 ToggleUnderline() 代替，如需要直接设置特殊装饰请使用 SetUnderline 方法直接代替")]
		void ToggleUnderline(TextDecorationCollection textDecoration);

		void ToggleUnderline();

		void ToggleUnderline(ISelection selection);

		void SetUnderline(TextDecorationCollection textDecoration);

		void ToggleStrikethrough(ISelection selection = null);

		void ToggleStrikethrough(ISegment segment);

		[Obsolete("此方法名是开启或关闭文本加粗，实际作用是设置文本加粗，实现和命名不同，请使用 SetFontWeight 替换")]
		void ToggleBold(FontWeight fontWeight);

		void SetFontWeight(FontWeight fontWeight);

		[Obsolete("此方法名是开启或关闭文本斜体但实际作为是设置字体样式，实现和命名不同，请使用 SetFontStyle 代替")]
		void ToggleItalic(FontStyle fontStyle);

		void SetFontStyle(FontStyle fontStyle);

		[Obsolete("此方法名是开启或关闭文本的下标，但实际上是设置文本的上标和下标在相同的方法，实现和命名不同，请使用 SetFontVariants 代替")]
		void ToggleSubscript(FontVariants fontVariants);

		[Obsolete("此方法名是开启或关闭文本的上标，但实际上是设置文本的上标和下标在相同的方法，实现和命名不同，请使用 SetFontVariants 代替")]
		void ToggleSuperscript(FontVariants fontVariants);

		void SetFontVariants(FontVariants fontVariants);

		void ApplyFormatPainter(TextEditorProperty formaTextEditorProperty);

		void SetMarker(MarkerStyle markerStyle);

		void SetLineSpace(double lineSpace);

		void DecreaseIndentation();

		void IncreaseIndentation();

		void AlignLeft();

		void AlignCenter();

		void AlignRight();

		void AlignJustify();

		void AlignTop();

		void AlignVerticalCenter();

		void AlignBottom();

		void Reset();

		void Select(ISelection selection);

		void MoveCaret(CaretOffset newOffset);

		void ChangeHyperlink(string hyperlink);

		void EditChars(ISegment segment, List<ICharObject> charObjects);

		void SetDirection(FlowDirection value);
	}
}
