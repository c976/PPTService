using System;
using System.ComponentModel;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public enum PropertyType
	{
		All,
		FontSize,
		FontFamily,
		Foreground,
		FontStyle,
		FontWeight,
		FontVariants,
		TextDecoration,
		EffectsArgs,
		Opacity,
		ParagraphBefore,
		ParagraphAfter,
		Direction,
		ArrangingType,
		MarkerStyle,
		LineSpacing,
		MarginLeft,
		TextAlignment,
		TextVerticalAlignment,
		[Obsolete("请使用 TextAlignment 代替")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		TextHoriztontalAlignment,
		Hyperlink,
		RunProperty,
		ParagraphProperty,
		DocumentProperty
	}
}
