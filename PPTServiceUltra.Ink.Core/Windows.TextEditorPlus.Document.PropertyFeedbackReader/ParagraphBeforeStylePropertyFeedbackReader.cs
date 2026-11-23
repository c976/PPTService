using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ParagraphBeforeStylePropertyFeedbackReader : SelectionParagraphPropertyFeedbackReader<double?>
	{
		public ParagraphBeforeStylePropertyFeedbackReader()
			: base((IEqualityComparer<double?>)new NullableDoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double? value)
		{
			feedbackProperty.ParagraphBefore = value;
		}

		protected override double? GetPropertyFromParagraphProperty(ParagraphProperty paragraphProperty)
		{
			return paragraphProperty.ParagraphBefore;
		}
	}
}
