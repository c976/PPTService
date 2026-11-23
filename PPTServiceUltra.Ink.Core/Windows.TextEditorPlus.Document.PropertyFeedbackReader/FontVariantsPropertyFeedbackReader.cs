using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class FontVariantsPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<FontVariants>
	{
		public FontVariantsPropertyFeedbackReader()
			: base((IEqualityComparer<FontVariants>)new FontVariantsEqualityComparer())
		{
		}

		protected override FontVariants GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.FontVariants;
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, FontVariants value)
		{
			feedbackProperty.FontVariants = value;
		}
	}
}
