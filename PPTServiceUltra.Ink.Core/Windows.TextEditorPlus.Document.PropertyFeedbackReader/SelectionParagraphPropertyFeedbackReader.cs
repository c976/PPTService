using System.Collections.Generic;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal abstract class SelectionParagraphPropertyFeedbackReader<T> : PropertyFeedbackReader<T>
	{
		protected SelectionParagraphPropertyFeedbackReader(IEqualityComparer<T> comparer)
			: base(comparer)
		{
		}

		protected abstract T GetPropertyFromParagraphProperty(ParagraphProperty paragraphProperty);

		protected sealed override T GetProperty(PropertyManager propertyManager, int offset)
		{
			return GetPropertyFromParagraphProperty(propertyManager.GetParagraphProperty(offset));
		}
	}
}
