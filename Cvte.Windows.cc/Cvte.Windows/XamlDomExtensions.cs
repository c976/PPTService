using System;
using System.Windows;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Windows
{
	public static class XamlDomExtensions
	{
		public static DependencyObject VisualAncestor([NotNull] this DependencyObject element, [NotNull] Predicate<DependencyObject> predicate)
		{
			return FindAncestor(element, VisualTreeHelper.GetParent, predicate);
		}

		public static T VisualAncestor<T>([NotNull] this DependencyObject element, [NotNull] Predicate<T> predicate) where T : DependencyObject
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return (T)element.VisualAncestor((DependencyObject x) =>
			{
				T val = x as T;
				return val != null && predicate(val);
			});
		}

		public static T VisualAncestor<T>([NotNull] this DependencyObject element) where T : DependencyObject
		{
			return (T)element.VisualAncestor((DependencyObject x) => x is T);
		}

		public static DependencyObject LogicalAncestor([NotNull] this DependencyObject element, [NotNull] Predicate<DependencyObject> predicate)
		{
			return FindAncestor(element, LogicalTreeHelper.GetParent, predicate);
		}

		public static T LogicalAncestor<T>([NotNull] this DependencyObject element, [NotNull] Predicate<T> predicate) where T : DependencyObject
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return (T)element.LogicalAncestor((DependencyObject x) =>
			{
				T val = x as T;
				return val != null && predicate(val);
			});
		}

		public static T LogicalAncestor<T>([NotNull] this DependencyObject element) where T : DependencyObject
		{
			return (T)element.LogicalAncestor((DependencyObject x) => x is T);
		}

		private static DependencyObject FindAncestor([NotNull] DependencyObject element, [NotNull] Func<DependencyObject, DependencyObject> parentSelector, [NotNull] Predicate<DependencyObject> predicate)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (parentSelector == null)
			{
				throw new ArgumentNullException("parentSelector");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			DependencyObject dependencyObject = parentSelector(element);
			while (true)
			{
				if (dependencyObject != null)
				{
					if (predicate(dependencyObject))
					{
						break;
					}
					dependencyObject = parentSelector(dependencyObject);
					continue;
				}
				return null;
			}
			return dependencyObject;
		}
	}
}
