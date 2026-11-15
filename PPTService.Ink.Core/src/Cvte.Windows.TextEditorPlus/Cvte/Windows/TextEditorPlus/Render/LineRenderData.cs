using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class LineRenderData
	{
		internal List<ICharVisual> CharVisuals { get; set; } = new List<ICharVisual>();

		public int CharVisualsCount => CharVisuals.Count;

		internal List<UnderlineRenderData> Underlines { get; set; } = new List<UnderlineRenderData>();

		internal List<AttachedDecorationData> AttachedDecorationDataList { get; set; } = new List<AttachedDecorationData>();

		internal List<UiElementCharVisual> EffectCharVisuals { get; set; } = new List<UiElementCharVisual>();

		internal List<List<int>> SamePropIndexes { get; set; } = new List<List<int>>();

		internal double Height { get; set; }

		public double OutlineHeight { get; set; }

		public double Top { get; set; }

		public double Bottom { get; set; }

		public double OutlineTop { get; set; }

		public double OutlineBottom { get; set; }

		internal double Width { get; set; }

		internal double TextWidth { get; set; }
	}
}
