using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class UIElementTranslateUtils
	{
		public static Rect TranslateRect(this UIElement source, Rect rect, UIElement target)
		{
			Point point = source.TranslatePoint(rect.TopLeft, target);
			Point point2 = source.TranslatePoint(rect.BottomRight, target);
			return new Rect(point, point2);
		}
	}
}
