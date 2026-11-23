using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class FontStylePropertyFeedbackReader : SelectionRunPropertyFeedbackReader<FontStyle>
	{
		protected override FontStyle DefaultValue { get; } = FontStyles.Normal;

		public FontStylePropertyFeedbackReader()
			: base((IEqualityComparer<FontStyle>)new FontStyleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, FontStyle value)
		{
			feedbackProperty.FontStyle = value;
		}

		protected override FontStyle GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.FontStyle;
		}
	}
}
