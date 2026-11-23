using System.Collections.Generic;
using System.Linq;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ShadowOpacityPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<double?>
	{
		public ShadowOpacityPropertyFeedbackReader()
			: base((IEqualityComparer<double?>)new NullableDoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double? value)
		{
			feedbackProperty.ShadowOpacity = value;
		}

		protected override double? GetPropertyFromRunProperty(RunProperty runProperty)
		{
			if (runProperty.EffectsArgs.ContainsType(typeof(ShadowEffectArgs)))
			{
				return runProperty.EffectsArgs.OfType<ShadowEffectArgs>().First().ShadowOpacity;
			}
			if (runProperty.EffectsArgs.ContainsType(typeof(PerspectiveEffectArgs)))
			{
				return runProperty.EffectsArgs.OfType<PerspectiveEffectArgs>().First().PerspectiveOpacity;
			}
			return null;
		}
	}
}
