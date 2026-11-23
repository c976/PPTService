using System.Collections.Generic;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class FontFamilyPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<FontFamily>
	{
		public FontFamilyPropertyFeedbackReader()
			: base((IEqualityComparer<FontFamily>)new FontFamilyEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, FontFamily value)
		{
			feedbackProperty.FontFamily = value;
		}

		protected override FontFamily GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.FontFamily;
		}
	}
}
