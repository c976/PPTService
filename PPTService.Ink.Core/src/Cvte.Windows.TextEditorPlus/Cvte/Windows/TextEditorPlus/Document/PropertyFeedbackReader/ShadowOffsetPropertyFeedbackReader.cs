using System.Collections.Generic;
using System.Linq;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ShadowOffsetPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<double>
	{
		public ShadowOffsetPropertyFeedbackReader()
			: base((IEqualityComparer<double>)new DoubleEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, double value)
		{
			feedbackProperty.ShadowOffset = value;
		}

		protected override double GetPropertyFromRunProperty(RunProperty runProperty)
		{
			if (runProperty.EffectsArgs.ContainsType(typeof(ShadowEffectArgs)))
			{
				return runProperty.EffectsArgs.OfType<ShadowEffectArgs>().First().ShadowDistance;
			}
			return 0.0;
		}
	}
}
