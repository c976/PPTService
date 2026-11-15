using System.Collections.Generic;
using System.Windows;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class UnderlinePropertyFeedbackReader : SelectionRunPropertyFeedbackReader<TextDecorationCollection>
	{
		protected override TextDecorationCollection DefaultValue => new TextDecorationCollection();

		public UnderlinePropertyFeedbackReader()
			: base((IEqualityComparer<TextDecorationCollection>)new TextDecorationCollectionEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, TextDecorationCollection value)
		{
			feedbackProperty.TextDecoration = value;
		}

		protected override TextDecorationCollection GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.TextDecoration;
		}
	}
}
