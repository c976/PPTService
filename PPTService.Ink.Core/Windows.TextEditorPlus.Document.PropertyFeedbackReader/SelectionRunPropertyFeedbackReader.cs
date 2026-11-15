using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal abstract class SelectionRunPropertyFeedbackReader<T> : PropertyFeedbackReader<T>
	{
		protected SelectionRunPropertyFeedbackReader(IEqualityComparer<T> comparer)
			: base(comparer)
		{
		}

		protected abstract T GetPropertyFromRunProperty(RunProperty runProperty);

		protected sealed override T GetProperty(PropertyManager propertyManager, int offset)
		{
			return GetPropertyFromRunProperty(propertyManager.GetRunProperty(offset));
		}
	}
}
