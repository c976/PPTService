using System;
using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Framework
{
	public static class OutlineExtensions
	{
		public static Rect GetOutlineExternalBounds([NotNull] this IEnumerable<Element> elements, bool includeOutlineThickness = true)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			Rect empty = Rect.Empty;
			foreach (Element element in elements)
			{
				empty.Union(element.Outline.GetExternalBounds(includeOutlineThickness));
			}
			return empty;
		}
	}
}
