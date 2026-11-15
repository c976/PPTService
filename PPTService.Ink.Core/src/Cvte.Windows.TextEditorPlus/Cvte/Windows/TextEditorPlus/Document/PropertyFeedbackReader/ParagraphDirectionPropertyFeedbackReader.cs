using System.Collections.Generic;
using System.Windows;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ParagraphDirectionPropertyFeedbackReader : SelectionParagraphPropertyFeedbackReader<FlowDirection>
	{
		public ParagraphDirectionPropertyFeedbackReader()
			: base((IEqualityComparer<FlowDirection>)new FlowDirectionEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, FlowDirection value)
		{
			feedbackProperty.ParagraphDirection = value;
		}

		protected override FlowDirection GetPropertyFromParagraphProperty(ParagraphProperty paragraphProperty)
		{
			return paragraphProperty.Direction;
		}
	}
}
