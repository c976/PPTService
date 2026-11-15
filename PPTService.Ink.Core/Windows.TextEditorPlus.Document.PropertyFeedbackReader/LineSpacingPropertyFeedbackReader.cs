using System.Collections.Generic;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class LineSpacingPropertyFeedbackReader : SelectionParagraphPropertyFeedbackReader<double?>
	{
		public LineSpacingPropertyFeedbackReader()
			: base((IEqualityComparer<double?>)new NullableDoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double? value)
		{
			feedbackProperty.LineSpacing = value;
		}

		protected override double? GetPropertyFromParagraphProperty(ParagraphProperty paragraphProperty)
		{
			return paragraphProperty.LineSpacing;
		}
	}
}
