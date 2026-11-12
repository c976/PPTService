using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Cvte.Paint.Framework.Level
{
	public static class LevelManager
	{
		internal static readonly DependencyProperty InitializedLevelPropertiesProperty = DependencyProperty.RegisterAttached("InitializedLevelProperties", typeof(InitializedLevelProperties), typeof(UIElement));

		public static void SetInitializedLevelProperties(this DependencyObject element, InitializedLevelProperties initializedLevelProperties)
		{
			element.SetValue(InitializedLevelPropertiesProperty, initializedLevelProperties);
		}

		public static InitializedLevelProperties GetInitializedLevelProperties(this DependencyObject element)
		{
			return (InitializedLevelProperties)element.GetValue(InitializedLevelPropertiesProperty);
		}

		public static void ClearInitializedLevelProperties(DependencyObject element)
		{
			element.ClearValue(InitializedLevelPropertiesProperty);
		}

		public static List<Element> GetLevelSortedElements(this ILevelContainer elementLevelContainer, IEnumerable<Element> elements)
		{
			return elementLevelContainer.LevelStorage.GetLevelSortedElements(elements).Cast<Element>().ToList();
		}

		public static List<Element> GetLevelSortedElements(this ILevelContainer elementLevelContainer)
		{
			return elementLevelContainer.LevelStorage.GetLevelSortedElements().Cast<Element>().ToList();
		}

		public static List<ILevelElement> GetLevelSortedElementList(this ILevelContainer elementLevelContainer)
		{
			return ((ILevelStorage)elementLevelContainer.LevelStorage).GetLevelSortedElements().ToList();
		}

		public static Element GetTopElement(this ILevelContainer levelContainer)
		{
			return levelContainer.GetLevelSortedElements().Last();
		}

		public static Element GetTopElement(this ILevelContainer levelContainer, IEnumerable<Element> elements)
		{
			return (Element)levelContainer.LevelStorage.GetLevelSortedElements(elements).Last();
		}
	}
}
