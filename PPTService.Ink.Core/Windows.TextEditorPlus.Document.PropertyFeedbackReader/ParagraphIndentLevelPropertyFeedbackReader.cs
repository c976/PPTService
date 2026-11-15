using System.Collections.Generic;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ParagraphIndentLevelPropertyFeedbackReader : SelectionParagraphPropertyFeedbackReader<int?>
	{
		public ParagraphIndentLevelPropertyFeedbackReader()
			: base((IEqualityComparer<int?>)new NullableIntEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, int? value)
		{
			feedbackProperty.IndentLevel = value;
		}

		protected override int? GetPropertyFromParagraphProperty(ParagraphProperty paragraphProperty)
		{
			return paragraphProperty.IndentLevel;
		}
	}
}
