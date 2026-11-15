using System;
using System.Windows;
using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Settings;
using PPTService.Paint.Settings;

namespace PPTService.Paint.Inks.Highlighter
{
	public class HighlighterPenMode : HardPenMode
	{
		private class HighlighterSegmentedRenderingDynamicInk : HardPenDynamicInk
		{
			public HighlighterSegmentedRenderingDynamicInk(UIElement adornedElement, InkBoardSettings state)
				: base(adornedElement, state)
			{
				Opacity = 0.3;
			}

			protected override Element ToInk(CombinedStroke stroke)
			{
				HighlighterInk highlighterInk = HighlighterInk.CreateHighlighterInkFromSlideStroke(stroke);
				highlighterInk.HighlighterOpacity = Opacity;
				return highlighterInk;
			}
		}

		private HighlighterWritingModeSettings HighlighterWritingModeSettings { get; set; }

		protected override DynamicInk CreateDynamicInk()
		{
			return new HighlighterSegmentedRenderingDynamicInk(CurrentSlide, InkBoardSettings)
			{
				InkId = Guid.NewGuid().ToString("N"),
				Opacity = HighlighterWritingModeSettings.HighlighterOpacity
			};
		}

		protected override void OnSwitchOn()
		{
			HighlighterWritingModeSettings = CurrentSlide.GetSettings<HighlighterWritingModeSettings>();
			base.OnSwitchOn();
		}
	}
}
