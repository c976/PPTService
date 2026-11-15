using System.Collections.Generic;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class OpacityPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<double?>
	{
		public OpacityPropertyFeedbackReader()
			: base((IEqualityComparer<double?>)new NullableDoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double? value)
		{
			feedbackProperty.Opacity = value;
		}

		protected override double? GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.Opacity;
		}
	}
}
