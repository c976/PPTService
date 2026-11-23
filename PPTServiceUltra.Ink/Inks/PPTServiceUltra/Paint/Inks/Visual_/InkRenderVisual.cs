using System.Collections.Generic;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Inks.Visual_
{
	internal static class InkRenderVisual
	{
		public static List<DrawingVisual> GetInkRenderDryingDrawVisual([NotNull] DynamicRenderer.InkRenderInfo inkRender)
		{
			return inkRender.StrokeVisualList.GetDryingDrawVisual();
		}
	}
}
