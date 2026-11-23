using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class TextMarkerStylePropertyFeedbackReader : SelectionParagraphPropertyFeedbackReader<MarkerStyle>
	{
		public TextMarkerStylePropertyFeedbackReader()
			: base((IEqualityComparer<MarkerStyle>)new MarkerStyleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, MarkerStyle value)
		{
			feedbackProperty.MarkerStyle = value;
		}

		protected override MarkerStyle GetPropertyFromParagraphProperty(ParagraphProperty paragraphProperty)
		{
			return paragraphProperty.MarkerProperty.ToMarkerStyle();
		}
	}
}
