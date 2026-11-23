using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class FontWeightPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<FontWeight>
	{
		protected override FontWeight DefaultValue { get; } = FontWeights.Normal;

		public FontWeightPropertyFeedbackReader()
			: base((IEqualityComparer<FontWeight>)new FontWeightEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, FontWeight value)
		{
			feedbackProperty.FontWeight = value;
		}

		protected override FontWeight GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.FontWeight;
		}
	}
}
