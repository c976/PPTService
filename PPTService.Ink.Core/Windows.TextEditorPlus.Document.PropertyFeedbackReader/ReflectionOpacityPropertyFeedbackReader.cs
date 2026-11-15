using System.Collections.Generic;
using System.Linq;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ReflectionOpacityPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<double?>
	{
		public ReflectionOpacityPropertyFeedbackReader()
			: base((IEqualityComparer<double?>)new NullableDoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double? value)
		{
			feedbackProperty.ReflectionOpacity = value;
		}

		protected override double? GetPropertyFromRunProperty(RunProperty runProperty)
		{
			if (runProperty.EffectsArgs.ContainsType(typeof(ReflectionEffectArgs)))
			{
				return runProperty.EffectsArgs.OfType<ReflectionEffectArgs>().First().ReflectionOpacity;
			}
			return null;
		}
	}
}
