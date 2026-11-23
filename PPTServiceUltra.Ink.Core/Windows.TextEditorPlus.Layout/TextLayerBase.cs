using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Layout
{
	internal class TextLayerBase : Layer
	{
		protected readonly List<LineVisualBase> Visuals = new List<LineVisualBase>();

		protected override int VisualChildrenCount => Visuals.Count;

		internal TextLayerBase(KnownLayer knownLayer)
			: base(knownLayer)
		{
		}

		public virtual void LayoutLineVisuals(List<ParagraphLayoutData> paragraphLayoutData, double editorWidth)
		{
			TextTracer.Debug("清空和移除旧文本行", "文本渲染");
			List<LineVisualBase> isAliveVisuals = paragraphLayoutData.SelectMany((ParagraphLayoutData x) => x.Lines.Where((LineVisualBase y) => y.IsAlive)).ToList();
			Visuals.Where((LineVisualBase x) => !isAliveVisuals.Contains(x)).ToList().ForEach(RemoveVisualChild);
			Visuals.Clear();
			paragraphLayoutData.ForEach((ParagraphLayoutData data) =>
			{
				data.Lines.ForEach((LineVisualBase line) =>
				{
					line.InvalidateArrange();
				});
			});
		}

		protected override Visual GetVisualChild(int index)
		{
			return Visuals[index];
		}

		internal void UpdateCacheMode(CacheMode cacheMode)
		{
			CacheMode = cacheMode;
			Visuals.ForEach((LineVisualBase x) =>
			{
				x.CacheMode = cacheMode;
			});
		}
	}
}
