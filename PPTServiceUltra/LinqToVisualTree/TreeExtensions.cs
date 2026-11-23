using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LinqToVisualTree
{
	public static class TreeExtensions
	{
		public static IEnumerable<DependencyObject> Descendants(this DependencyObject item)
		{
			ILinqTree<DependencyObject> linqTree = new VisualTreeAdapter(item);
			foreach (DependencyObject child in linqTree.Children())
			{
				yield return child;
				foreach (DependencyObject item2 in child.Descendants())
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<DependencyObject> DescendantsAndSelf(this DependencyObject item)
		{
			yield return item;
			foreach (DependencyObject item2 in item.Descendants())
			{
				yield return item2;
			}
		}

		public static IEnumerable<DependencyObject> Ancestors(this DependencyObject item)
		{
			ILinqTree<DependencyObject> linqTree = new VisualTreeAdapter(item);
			for (DependencyObject parent = linqTree.Parent; parent != null; parent = linqTree.Parent)
			{
				yield return parent;
				linqTree = new VisualTreeAdapter(parent);
			}
		}

		public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject item)
		{
			yield return item;
			foreach (DependencyObject item2 in item.Ancestors())
			{
				yield return item2;
			}
		}

		public static IEnumerable<DependencyObject> Elements(this DependencyObject item)
		{
			ILinqTree<DependencyObject> linqTree = new VisualTreeAdapter(item);
			foreach (DependencyObject item2 in linqTree.Children())
			{
				yield return item2;
			}
		}

		public static IEnumerable<DependencyObject> ElementsBeforeSelf(this DependencyObject item)
		{
			if (item.Ancestors().FirstOrDefault() == null)
			{
				yield break;
			}
			foreach (DependencyObject item2 in item.Ancestors().First().Elements())
			{
				if (item2.Equals(item))
				{
					break;
				}
				yield return item2;
			}
		}

		public static IEnumerable<DependencyObject> ElementsAfterSelf(this DependencyObject item)
		{
			if (item.Ancestors().FirstOrDefault() == null)
			{
				yield break;
			}
			bool afterSelf = false;
			foreach (DependencyObject child in item.Ancestors().First().Elements())
			{
				if (afterSelf)
				{
					yield return child;
				}
				if (child.Equals(item))
				{
					afterSelf = true;
				}
			}
		}

		public static IEnumerable<DependencyObject> ElementsAndSelf(this DependencyObject item)
		{
			yield return item;
			foreach (DependencyObject item2 in item.Elements())
			{
				yield return item2;
			}
		}

		public static IEnumerable<DependencyObject> Descendants<T>(this DependencyObject item)
		{
			return (from i in item.Descendants()
				where i is T
				select i).Cast<DependencyObject>();
		}

		public static IEnumerable<DependencyObject> ElementsBeforeSelf<T>(this DependencyObject item)
		{
			return (from i in item.ElementsBeforeSelf()
				where i is T
				select i).Cast<DependencyObject>();
		}

		public static IEnumerable<DependencyObject> ElementsAfterSelf<T>(this DependencyObject item)
		{
			return (from i in item.ElementsAfterSelf()
				where i is T
				select i).Cast<DependencyObject>();
		}

		public static IEnumerable<DependencyObject> DescendantsAndSelf<T>(this DependencyObject item)
		{
			return (from i in item.DescendantsAndSelf()
				where i is T
				select i).Cast<DependencyObject>();
		}

		public static IEnumerable<DependencyObject> Ancestors<T>(this DependencyObject item)
		{
			return (from i in item.Ancestors()
				where i is T
				select i).Cast<DependencyObject>();
		}

		public static IEnumerable<DependencyObject> AncestorsAndSelf<T>(this DependencyObject item)
		{
			return (from i in item.AncestorsAndSelf()
				where i is T
				select i).Cast<DependencyObject>();
		}

		public static IEnumerable<DependencyObject> Elements<T>(this DependencyObject item)
		{
			return (from i in item.Elements()
				where i is T
				select i).Cast<DependencyObject>();
		}

		public static IEnumerable<DependencyObject> ElementsAndSelf<T>(this DependencyObject item)
		{
			return (from i in item.ElementsAndSelf()
				where i is T
				select i).Cast<DependencyObject>();
		}
	}
}
