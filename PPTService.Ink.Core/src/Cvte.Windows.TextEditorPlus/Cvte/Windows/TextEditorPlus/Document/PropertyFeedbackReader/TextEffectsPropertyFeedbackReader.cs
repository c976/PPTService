using System.Collections.Generic;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class TextEffectsPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<EffectArgsCollection>
	{
		public TextEffectsPropertyFeedbackReader()
			: base((IEqualityComparer<EffectArgsCollection>)new EffectArgsCollectionEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, EffectArgsCollection value)
		{
			feedbackProperty.TextEffects = value;
		}

		protected override EffectArgsCollection GetPropertyFromRunProperty(RunProperty runProperty)
		{
			return runProperty.EffectsArgs;
		}
	}
}
