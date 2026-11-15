using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader.Comparer;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document.PropertyFeedbackReader
{
	internal class ShadowBrushPropertyFeedbackReader : SelectionRunPropertyFeedbackReader<Brush>
	{
		public ShadowBrushPropertyFeedbackReader()
			: base((IEqualityComparer<Brush>)new BrushEqualityComparer())
		{
		}

		protected override void SetFeedbackProperty(FeedbackProperty feedbackProperty, Brush value)
		{
			feedbackProperty.ShadowBrush = value;
		}

		protected override Brush GetPropertyFromRunProperty(RunProperty runProperty)
		{
			if (runProperty.EffectsArgs.ContainsType(typeof(ShadowEffectArgs)))
			{
				return runProperty.EffectsArgs.OfType<ShadowEffectArgs>().First().ShadowBrush;
			}
			if (runProperty.EffectsArgs.ContainsType(typeof(PerspectiveEffectArgs)))
			{
				return runProperty.EffectsArgs.OfType<PerspectiveEffectArgs>().First().PerspectiveBrush;
			}
			return null;
		}
	}
}
