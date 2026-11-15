using System.Collections.Generic;
using System.Windows;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class TextAlignmentPropertyFeedbackReader : SelectionParagraphPropertyFeedbackReader<TextAlignment>
	{
		public TextAlignmentPropertyFeedbackReader()
			: base((IEqualityComparer<TextAlignment>)new TextAlignmentEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, TextAlignment value)
		{
			feedbackProperty.TextHorizontalAlignment = value.ToTextHorizontalAlignment();
		}

		protected override TextAlignment GetPropertyFromParagraphProperty(ParagraphProperty paragraphProperty)
		{
			return paragraphProperty.TextAlignment;
		}
	}
}
