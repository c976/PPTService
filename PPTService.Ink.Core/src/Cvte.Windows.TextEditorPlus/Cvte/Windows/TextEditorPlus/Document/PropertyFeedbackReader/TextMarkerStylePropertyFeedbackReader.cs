using System.Collections.Generic;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
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
