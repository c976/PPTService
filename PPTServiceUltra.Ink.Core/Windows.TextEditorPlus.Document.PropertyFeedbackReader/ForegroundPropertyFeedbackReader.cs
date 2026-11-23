using System.Collections.Generic;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ForegroundPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<Brush>
	{
		public ForegroundPropertyFeedbackReader()
			: base((IEqualityComparer<Brush>)new BrushEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, Brush value)
		{
			feedbackProperty.Foreground = value;
		}

		protected override Brush GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.Foreground;
		}
	}
}
