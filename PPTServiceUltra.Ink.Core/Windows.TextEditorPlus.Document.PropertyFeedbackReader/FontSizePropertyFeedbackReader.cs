using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class FontSizePropertyFeedbackReader : SelectionRunPropertyFeedbackReader<double?>
	{
		public FontSizePropertyFeedbackReader()
			: base((IEqualityComparer<double?>)new NullableDoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double? value)
		{
			feedbackProperty.FontSize = value;
		}

		protected override double? GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.FontSize;
		}
	}
}
