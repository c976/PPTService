using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class TextEditorHelper
	{
		public static List<Rect> OptimizeRects(List<Rect> rects)
		{
			if (rects.Count > 1)
			{
				return VerticalOptimizeRects(HorizontalOptimizeRects(rects));
			}
			return rects;
		}

		private static List<Rect> HorizontalOptimizeRects(List<Rect> rects)
		{
			if (rects.Count <= 1)
			{
				return rects;
			}
			List<Rect> list = new List<Rect>();
			double top = rects.First().Top;
			int index = 0;
			for (int i = 1; i < rects.Count; i++)
			{
				if (!(Math.Abs(rects[i].Top - top) < double.Epsilon))
				{
					list.Add(Rect.Union(rects[index], rects[i - 1]));
					index = i;
					top = rects[i].Top;
				}
			}
			list.Add(Rect.Union(rects[index], rects[rects.Count - 1]));
			return list;
		}

		private static List<Rect> VerticalOptimizeRects(List<Rect> rects)
		{
			if (rects.Count <= 1)
			{
				return rects;
			}
			List<Rect> list = new List<Rect>();
			double left = rects.First().Left;
			double right = rects.First().Right;
			int index = 0;
			for (int i = 1; i < rects.Count; i++)
			{
				if (!(Math.Abs(rects[i].Left - left) < double.Epsilon) || !(Math.Abs(rects[i].Right - right) < double.Epsilon))
				{
					list.Add(Rect.Union(rects[index], rects[i - 1]));
					index = i;
					left = rects[i].Left;
					right = rects[i].Right;
				}
			}
			list.Add(Rect.Union(rects[index], rects[rects.Count - 1]));
			return list;
		}

		public static UIElementContainer GetUIElementContainer(Rect underlineBounds, EffectArgsCollection effectArgs, Brush foreground)
		{
			UIElementContainer uIElementContainer = new UIElementContainer();
			List<DrawingVisualContainer> textEffectContainers = TextEffectFactory.GetTextEffectContainers(new List<TextCharVisual>
			{
				new TextCharVisual
				{
					TextGeometry = Geometry.Empty,
					Direction = CharFlowDirection.Neutral
				}
			}, effectArgs, DrawType.Underline, new UnderlineRenderData
			{
				DashStyle = DashStyles.Solid,
				Foreground = foreground,
				UnderlineBounds = underlineBounds
			}, underlineBounds.Bottom);
			uIElementContainer.AddChildren(textEffectContainers);
			return uIElementContainer;
		}
	}
}
