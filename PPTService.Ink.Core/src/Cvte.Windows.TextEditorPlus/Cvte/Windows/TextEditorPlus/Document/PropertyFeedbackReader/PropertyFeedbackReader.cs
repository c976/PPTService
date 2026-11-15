using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal abstract class PropertyFeedbackReader<T> : IPropertyFeedbackReader
	{
		protected virtual T DefaultValue { get; }

		private IEqualityComparer<T> Comparer { get; }

		protected PropertyFeedbackReader(IEqualityComparer<T> comparer)
		{
			Comparer = comparer;
		}

		public T GetSelectionProperty(TextEditor textEditor, int frontOffset, int backOffset)
		{
			PropertyManager propertyManager = textEditor.DocumentManager.PropertyManager;
			T property = GetProperty(propertyManager, backOffset);
			for (int i = frontOffset + 1; i < backOffset; i++)
			{
				T property2 = GetProperty(propertyManager, i);
				if (!Comparer.Equals(property2, property))
				{
					return DefaultValue;
				}
			}
			return property;
		}

		public FeedbackProperty GetSelectionPropertyToFeedbackProperty(TextEditor textEditor, int frontOffset, int backOffset, FeedbackProperty feedbackProperty)
		{
			T selectionProperty = GetSelectionProperty(textEditor, frontOffset, backOffset);
			SetFeedbackProperty(feedbackProperty, selectionProperty);
			return feedbackProperty;
		}

		protected abstract void SetFeedbackProperty(FeedbackProperty feedbackProperty, T value);

		protected abstract T GetProperty(PropertyManager propertyManager, int offset);
	}
}
