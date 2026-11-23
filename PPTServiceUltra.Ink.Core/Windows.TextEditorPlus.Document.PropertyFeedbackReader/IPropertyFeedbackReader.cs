namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal interface IPropertyFeedbackReader
	{
		FeedbackProperty GetSelectionPropertyToFeedbackProperty(TextEditor textEditor, int frontOffset, int backOffset, FeedbackProperty feedbackProperty);
	}
}
