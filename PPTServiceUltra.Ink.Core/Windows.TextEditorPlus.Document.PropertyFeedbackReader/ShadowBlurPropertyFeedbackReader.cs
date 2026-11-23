using System.Collections.Generic;
using System.Linq;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ShadowBlurPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<double?>
	{
		protected override double? DefaultValue { get; } = 0.0;

		public ShadowBlurPropertyFeedbackReader()
			: base((IEqualityComparer<double?>)new NullableDoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double? value)
		{
			feedbackProperty.ShadowBlur = value.GetValueOrDefault();
		}

		protected override double? GetPropertyFromRunProperty(RunProperty runProperty)
		{
			EffectArgsCollection effectsArgs = runProperty.EffectsArgs;
			if (effectsArgs.ContainsType(typeof(ShadowEffectArgs)))
			{
				return effectsArgs.OfType<ShadowEffectArgs>().First().ShadowBlur;
			}
			if (effectsArgs.ContainsType(typeof(PerspectiveEffectArgs)))
			{
				return effectsArgs.OfType<PerspectiveEffectArgs>().First().PerspectiveBlur;
			}
			return 0.0;
		}
	}
}
