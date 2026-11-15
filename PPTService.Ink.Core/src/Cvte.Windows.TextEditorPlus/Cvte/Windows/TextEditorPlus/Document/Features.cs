namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class Features
	{
		internal static readonly ReadOnlyFeature SetStyleFeatureInner;

		internal static readonly ReadOnlyFeature CcpFeatureInner;

		internal static readonly ReadOnlyFeature EditFeatureInner;

		internal static readonly ReadOnlyFeature HyperlinkFeatureInner;

		internal static readonly ReadOnlyFeature AlignVerticalFeatureInner;

		internal static readonly ReadOnlyFeature FullFeatureInner;

		internal static readonly SingleFeature IncreaseFontSizeInner;

		internal static readonly SingleFeature DecreaseFontSizeInner;

		internal static readonly SingleFeature SetFontSizeInner;

		internal static readonly SingleFeature SetFontFamilyInner;

		internal static readonly SingleFeature SetForegroundInner;

		internal static readonly SingleFeature SetTextEffectsInner;

		internal static readonly SingleFeature ToggleUnderlineInner;

		public static readonly SingleFeature ToggleStrikethrough;

		internal static readonly SingleFeature ToggleBoldInner;

		internal static readonly SingleFeature ToggleItalicInner;

		internal static readonly SingleFeature SetFontSubscriptInner;

		internal static readonly SingleFeature SetFontSuperscriptInner;

		internal static readonly SingleFeature ApplyFormatPainterInner;

		internal static readonly SingleFeature SetMarkerInner;

		internal static readonly SingleFeature SetLineSpaceInner;

		internal static readonly SingleFeature SetLineSpacingStrategyInner;

		internal static readonly SingleFeature DecreaseIndentationInner;

		internal static readonly SingleFeature IncreaseIndentationInner;

		internal static readonly SingleFeature AlignLeftInner;

		internal static readonly SingleFeature AlignCenterInner;

		internal static readonly SingleFeature AlignRightInner;

		internal static readonly SingleFeature AlignJustifyInner;

		internal static readonly SingleFeature AlignTopInner;

		internal static readonly SingleFeature AlignVerticalCenterInner;

		internal static readonly SingleFeature AlignBottomInner;

		internal static readonly SingleFeature ResetInner;

		internal static readonly SingleFeature SetParagraphSpaceBeforeInner;

		internal static readonly SingleFeature SetParagraphSpaceAfterInner;

		internal static readonly SingleFeature SetDirectionInner;

		internal static readonly SingleFeature EditTextInner;

		internal static readonly SingleFeature SelectInner;

		internal static readonly SingleFeature DragTextInner;

		internal static readonly SingleFeature CaretAnimationInner;

		internal static readonly SingleFeature CaretInner;

		internal static readonly SingleFeature ChangeHyperlinkInner;

		internal static readonly SingleFeature ContextMenuOpenHyperlinkInner;

		internal static readonly SingleFeature HyperlinkMouseHotInner;

		internal static readonly SingleFeature CopyInner;

		internal static readonly SingleFeature CutInner;

		internal static readonly SingleFeature PasteInner;

		internal static readonly SingleFeature ArrangingTypeInner;

		public static Feature SetStyleFeature => SetStyleFeatureInner.Clone();

		public static Feature CcpFeature => CcpFeatureInner.Clone();

		public static Feature EditFeature => EditFeatureInner.Clone();

		public static Feature HyperlinkFeature => HyperlinkFeatureInner.Clone();

		public static Feature AlignVerticalFeature => AlignVerticalFeatureInner.Clone();

		public static Feature FullFeature => FullFeatureInner.Clone();

		public static Feature IncreaseFontSize => IncreaseFontSizeInner.Clone();

		public static Feature DecreaseFontSize => DecreaseFontSizeInner.Clone();

		public static Feature SetFontSize => SetFontSizeInner.Clone();

		public static Feature SetFontFamily => SetFontFamilyInner.Clone();

		public static Feature SetForeground => SetForegroundInner.Clone();

		public static Feature SetTextEffects => SetTextEffectsInner.Clone();

		public static Feature ToggleUnderline => ToggleUnderlineInner.Clone();

		public static Feature ToggleBold => ToggleBoldInner.Clone();

		public static Feature ToggleItalic => ToggleItalicInner.Clone();

		public static Feature SetFontSubscript => SetFontSubscriptInner.Clone();

		public static Feature SetFontSuperscript => SetFontSuperscriptInner.Clone();

		public static Feature ApplyFormatPainter => ApplyFormatPainterInner.Clone();

		public static Feature SetMarker => SetMarkerInner.Clone();

		public static Feature SetLineSpace => SetLineSpaceInner.Clone();

		public static Feature SetLineSpacingStrategy => SetLineSpacingStrategyInner.Clone();

		public static Feature DecreaseIndentation => DecreaseIndentationInner.Clone();

		public static Feature IncreaseIndentation => IncreaseIndentationInner.Clone();

		public static Feature AlignLeft => AlignLeftInner.Clone();

		public static Feature AlignCenter => AlignCenterInner.Clone();

		public static Feature AlignRight => AlignRightInner.Clone();

		public static Feature AlignJustify => AlignJustifyInner.Clone();

		public static Feature AlignTop => AlignTopInner.Clone();

		public static Feature AlignVerticalCenter => AlignVerticalCenterInner.Clone();

		public static Feature AlignBottom => AlignBottomInner.Clone();

		public static Feature Reset => ResetInner.Clone();

		public static Feature SetParagraphSpaceBefore => SetParagraphSpaceBeforeInner.Clone();

		public static Feature SetParagraphSpaceAfter => SetParagraphSpaceAfterInner.Clone();

		public static Feature EditText => EditTextInner.Clone();

		public static Feature Select => SelectInner.Clone();

		public static Feature DragText => DragTextInner.Clone();

		public static Feature CaretAnimation => CaretAnimationInner.Clone();

		public static Feature Caret => CaretInner.Clone();

		public static Feature ChangeHyperlink => ChangeHyperlinkInner.Clone();

		public static Feature ContextMenuOpenHyperlink => ContextMenuOpenHyperlinkInner.Clone();

		public static Feature HyperlinkMouseHot => HyperlinkMouseHotInner.Clone();

		public static Feature Copy => CopyInner.Clone();

		public static Feature Cut => CutInner.Clone();

		public static Feature Paste => PasteInner.Clone();

		public static Feature ArrangingType => ArrangingTypeInner.Clone();

		public static Feature SetDirection => SetDirectionInner.Clone();

		static Features()
		{
			IncreaseFontSizeInner = new SingleFeature("IncreaseFontSize");
			DecreaseFontSizeInner = new SingleFeature("DecreaseFontSize");
			SetFontSizeInner = new SingleFeature("SetFontSize");
			SetFontFamilyInner = new SingleFeature("SetFontFamily");
			SetForegroundInner = new SingleFeature("SetForeground");
			SetTextEffectsInner = new SingleFeature("SetTextEffects");
			ToggleUnderlineInner = new SingleFeature("ToggleUnderline");
			ToggleStrikethrough = new SingleFeature("ToggleStrikethrough");
			ToggleBoldInner = new SingleFeature("ToggleBold");
			ToggleItalicInner = new SingleFeature("ToggleItalic");
			SetFontSubscriptInner = new SingleFeature("ToggleSubscript");
			SetFontSuperscriptInner = new SingleFeature("ToggleSuperscript");
			ApplyFormatPainterInner = new SingleFeature("ApplyFormatPainter");
			SetMarkerInner = new SingleFeature("SetMarker");
			SetLineSpaceInner = new SingleFeature("SetLineSpace");
			SetLineSpacingStrategyInner = new SingleFeature("SetLineSpacingStrategy");
			DecreaseIndentationInner = new SingleFeature("DecreaseIndentation");
			IncreaseIndentationInner = new SingleFeature("IncreaseIndentation");
			AlignLeftInner = new SingleFeature("AlignLeft");
			AlignCenterInner = new SingleFeature("AlignCenter");
			AlignRightInner = new SingleFeature("AlignRight");
			AlignJustifyInner = new SingleFeature("AlignJustify");
			AlignTopInner = new SingleFeature("AlignTop");
			AlignVerticalCenterInner = new SingleFeature("AlignVerticalCenter");
			AlignBottomInner = new SingleFeature("AlignBottom");
			ResetInner = new SingleFeature("Reset");
			SetParagraphSpaceBeforeInner = new SingleFeature("SetParagraphSpaceBefore");
			SetParagraphSpaceAfterInner = new SingleFeature("SetParagraphSpaceAfter");
			SetDirectionInner = new SingleFeature("SetDirection");
			EditTextInner = new SingleFeature("EditChars");
			SelectInner = new SingleFeature("Select");
			DragTextInner = new SingleFeature("DragText");
			CaretAnimationInner = new SingleFeature("CaretAnimation");
			CaretInner = new SingleFeature("MoveCaret");
			ChangeHyperlinkInner = new SingleFeature("ChangeHyperlink");
			ContextMenuOpenHyperlinkInner = new SingleFeature("ContextMenuOpenHyperlink");
			HyperlinkMouseHotInner = new SingleFeature("HyperlinkMouseHot");
			CopyInner = new SingleFeature("Copy");
			CutInner = new SingleFeature("Cut");
			PasteInner = new SingleFeature("Paste");
			ArrangingTypeInner = new SingleFeature("SetTextArrangingType");
			AlignVerticalFeatureInner = new ReadOnlyFeature(AlignTopInner, AlignVerticalCenterInner, AlignBottomInner);
			SetStyleFeatureInner = new ReadOnlyFeature(new ReadOnlyFeature[1] { AlignVerticalFeatureInner }, IncreaseFontSizeInner, DecreaseFontSizeInner, SetFontSizeInner, SetFontFamilyInner, SetForegroundInner, SetTextEffectsInner, ToggleUnderlineInner, ToggleStrikethrough, ToggleBoldInner, ToggleItalicInner, SetFontSubscriptInner, SetFontSuperscriptInner, ApplyFormatPainterInner, SetLineSpacingStrategyInner, SetDirectionInner, SetMarkerInner, SetLineSpaceInner, DecreaseIndentationInner, IncreaseIndentationInner, AlignLeftInner, AlignCenterInner, AlignRightInner, AlignJustifyInner, ResetInner, SetParagraphSpaceBeforeInner, SetParagraphSpaceAfterInner);
			CcpFeatureInner = new ReadOnlyFeature(CopyInner, CutInner, PasteInner);
			EditFeatureInner = new ReadOnlyFeature(EditTextInner, DragTextInner, CaretAnimationInner, CaretInner, SelectInner);
			HyperlinkFeatureInner = new ReadOnlyFeature(ChangeHyperlinkInner, HyperlinkMouseHotInner, ContextMenuOpenHyperlinkInner);
			FullFeatureInner = new ReadOnlyFeature(SetStyleFeatureInner, CcpFeatureInner, EditFeatureInner, HyperlinkFeatureInner, ArrangingTypeInner);
		}
	}
}
