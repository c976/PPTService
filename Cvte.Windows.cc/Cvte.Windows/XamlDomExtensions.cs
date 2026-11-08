using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Windows
{
	public static class XamlDomExtensions
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1<SA_T0> where SA_T0 : DependencyObject
		{
			public Predicate<SA_T0> predicate;

			public bool _003CVisualAncestor_003Eb__0(DependencyObject x)
			{
				if (4 != 0)
				{
					SA_T0 val = x as SA_T0;
					if (val != null && 0 == 0)
					{
						return predicate(val);
					}
				}
				return false;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5<SA_T0> where SA_T0 : DependencyObject
		{
			public Predicate<SA_T0> predicate;

			public bool _003CLogicalAncestor_003Eb__4(DependencyObject x)
			{
				if (4 != 0)
				{
					SA_T0 val = x as SA_T0;
					if (val != null && 0 == 0)
					{
						return predicate(val);
					}
				}
				return false;
			}
		}

		public static DependencyObject VisualAncestor([NotNull] this DependencyObject element, [NotNull] Predicate<DependencyObject> predicate)
		{
			return FindAncestor(element, VisualTreeHelper.GetParent, predicate);
		}

		public static T VisualAncestor<T>([NotNull] this DependencyObject element, [NotNull] Predicate<T> predicate) where T : DependencyObject
		{
			_003C_003Ec__DisplayClass1<T> obj2 = default(_003C_003Ec__DisplayClass1<T>);
			while (true)
			{
				_003C_003Ec__DisplayClass1<T> obj = new _003C_003Ec__DisplayClass1<T>();
				if (0 == 0)
				{
					obj2 = obj;
				}
				while (0 == 0)
				{
					obj2.predicate = predicate;
					if (4 != 0)
					{
						if (obj2.predicate == null)
						{
							throw new ArgumentNullException("predicate");
						}
						return (T)element.VisualAncestor(obj2._003CVisualAncestor_003Eb__0);
					}
				}
			}
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
			_003C_003Ec__DisplayClass5<T> obj2 = default(_003C_003Ec__DisplayClass5<T>);
			while (true)
			{
				_003C_003Ec__DisplayClass5<T> obj = new _003C_003Ec__DisplayClass5<T>();
				if (0 == 0)
				{
					obj2 = obj;
				}
				while (0 == 0)
				{
					obj2.predicate = predicate;
					if (4 != 0)
					{
						if (obj2.predicate == null)
						{
							throw new ArgumentNullException("predicate");
						}
						return (T)element.LogicalAncestor(obj2._003CLogicalAncestor_003Eb__4);
					}
				}
			}
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
			while (true)
			{
				DependencyObject dependencyObject = parentSelector(element);
				while (true)
				{
					if (dependencyObject != null)
					{
						if (predicate(dependencyObject))
						{
							return dependencyObject;
						}
						if (5 == 0)
						{
							break;
						}
						dependencyObject = parentSelector(dependencyObject);
						continue;
					}
					return null;
				}
			}
		}
	}
}
