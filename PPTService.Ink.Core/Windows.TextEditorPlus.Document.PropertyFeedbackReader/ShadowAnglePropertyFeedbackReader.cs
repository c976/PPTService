using System.Collections.Generic;
using System.Linq;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ShadowAnglePropertyFeedbackReader : SelectionRunPropertyFeedbackReader<double?>
	{
		protected override double? DefaultValue { get; } = 0.0;

		public ShadowAnglePropertyFeedbackReader()
			: base((IEqualityComparer<double?>)new NullableDoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double? value)
		{
			feedbackProperty.ShadowAngle = value;
		}

		protected override double? GetPropertyFromRunProperty(RunProperty runProperty)
		{
			EffectArgsCollection effectsArgs = runProperty.EffectsArgs;
			if (effectsArgs.ContainsType(typeof(ShadowEffectArgs)))
			{
				return effectsArgs.OfType<ShadowEffectArgs>().First().ShadowDirection;
			}
			return null;
		}
	}
}
