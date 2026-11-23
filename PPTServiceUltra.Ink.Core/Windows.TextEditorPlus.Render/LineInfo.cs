using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class LineInfo
	{
		internal Rect DefaultRunBounds { get; set; }

		internal double Baseline { get; set; }

		internal double RenderLineSpace { get; set; }

		internal List<ICharVisual> CharVisuals { get; set; } = new List<ICharVisual>();

		public int CharVisualsCount => CharVisuals.Count;

		internal LineProperty LineProperty { get; set; } = new LineProperty();

		internal List<List<int>> SamePropIndexes { get; set; } = new List<List<int>>();

		internal Dictionary<int, List<UnderlineRenderData>> UnderLines { get; set; } = new Dictionary<int, List<UnderlineRenderData>>();

		internal List<AttachedDecorationData> AttachedDecorationDataList { get; } = new List<AttachedDecorationData>();

		internal Dictionary<int, double> ScriptBaseLines { get; set; } = new Dictionary<int, double>();

		public override string ToString()
		{
			return string.Join("", CharVisuals);
		}
	}
}
