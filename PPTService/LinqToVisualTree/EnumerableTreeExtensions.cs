using System;
using System.Collections.Generic;
using System.Windows;

namespace LinqToVisualTree
{
	public static class EnumerableTreeExtensions
	{
		private static IEnumerable<DependencyObject> DrillDown(this IEnumerable<DependencyObject> items, Func<DependencyObject, IEnumerable<DependencyObject>> function)
		{
			foreach (DependencyObject item in items)
			{
				foreach (DependencyObject item2 in function(item))
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<DependencyObject> DrillDown<T>(this IEnumerable<DependencyObject> items, Func<DependencyObject, IEnumerable<DependencyObject>> function) where T : DependencyObject
		{
			foreach (DependencyObject item in items)
			{
				foreach (DependencyObject item2 in function(item))
				{
					if (item2 is T)
					{
						yield return (T)item2;
					}
				}
			}
		}

		public static IEnumerable<DependencyObject> Descendants(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown((DependencyObject i) => i.Descendants());
		}

		public static IEnumerable<DependencyObject> DescendantsAndSelf(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown((DependencyObject i) => i.DescendantsAndSelf());
		}

		public static IEnumerable<DependencyObject> Ancestors(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown((DependencyObject i) => i.Ancestors());
		}

		public static IEnumerable<DependencyObject> AncestorsAndSelf(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown((DependencyObject i) => i.AncestorsAndSelf());
		}

		public static IEnumerable<DependencyObject> Elements(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown((DependencyObject i) => i.Elements());
		}

		public static IEnumerable<DependencyObject> ElementsAndSelf(this IEnumerable<DependencyObject> items)
		{
			return items.DrillDown((DependencyObject i) => i.ElementsAndSelf());
		}

		public static IEnumerable<DependencyObject> Descendants<T>(this IEnumerable<DependencyObject> items) where T : DependencyObject
		{
			return items.DrillDown<T>((DependencyObject i) => i.Descendants());
		}

		public static IEnumerable<DependencyObject> DescendantsAndSelf<T>(this IEnumerable<DependencyObject> items) where T : DependencyObject
		{
			return items.DrillDown<T>((DependencyObject i) => i.DescendantsAndSelf());
		}

		public static IEnumerable<DependencyObject> Ancestors<T>(this IEnumerable<DependencyObject> items) where T : DependencyObject
		{
			return items.DrillDown<T>((DependencyObject i) => i.Ancestors());
		}

		public static IEnumerable<DependencyObject> AncestorsAndSelf<T>(this IEnumerable<DependencyObject> items) where T : DependencyObject
		{
			return items.DrillDown<T>((DependencyObject i) => i.AncestorsAndSelf());
		}

		public static IEnumerable<DependencyObject> Elements<T>(this IEnumerable<DependencyObject> items) where T : DependencyObject
		{
			return items.DrillDown<T>((DependencyObject i) => i.Elements());
		}

		public static IEnumerable<DependencyObject> ElementsAndSelf<T>(this IEnumerable<DependencyObject> items) where T : DependencyObject
		{
			return items.DrillDown<T>((DependencyObject i) => i.ElementsAndSelf());
		}
	}
}
