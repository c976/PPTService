using System.Collections.Generic;
using System.Windows.Media;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace Cvte.Paint.Inks.Visual_
{
	internal static class InkRenderVisual
	{
		public static List<DrawingVisual> GetInkRenderDryingDrawVisual([NotNull] DynamicRenderer.InkRenderInfo inkRender)
		{
			return inkRender.StrokeVisualList.GetDryingDrawVisual();
		}
	}
}
