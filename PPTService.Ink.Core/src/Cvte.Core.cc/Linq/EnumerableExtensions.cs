using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Linq
{
	
	
	public static class EnumerableExtensions
	{
		public static bool AllAny< T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			bool result = false;
			foreach (T item in source)
			{
				if (predicate(item))
				{
					result = true;
					continue;
				}
				return false;
			}
			return result;
		}

		public static bool AnyIntersect< T>(this List<T> a, List<T> b)
		{
			if (a == null || b == null || !a.Any() || !b.Any())
			{
				return false;
			}
			if (a.Count <= 10 && b.Count <= 10)
			{
				return a.Any(b.Contains);
			}
			if (a.Count < b.Count)
			{
				return ((IEnumerable<T>)b).AnyIntersect((IEnumerable<T>)a);
			}
			return ((IEnumerable<T>)a).AnyIntersect((IEnumerable<T>)b);
		}

		public static bool AnyIntersect< T>(this IEnumerable<T> a, IEnumerable<T> b)
		{
			return a.Intersect(b).Any();
		}

		public static bool SequenceEqualIgnoringOrder< T>(this ICollection<T> item1, ICollection<T> item2)
		{
			if (item1 == null)
			{
				throw new ArgumentNullException("item1");
			}
			if (item2 == null)
			{
				throw new ArgumentNullException("item2");
			}
			if (item1.Count == item2.Count)
			{
				return item1.All(item2.Contains);
			}
			return false;
		}

		public static bool SequenceContains< T>(this ICollection<T> item1, ICollection<T> item2)
		{
			if (item1 == null)
			{
				throw new ArgumentNullException("item1");
			}
			if (item2 == null)
			{
				throw new ArgumentNullException("item2");
			}
			if (item1.Count > item2.Count)
			{
				return item2.All(item1.Contains);
			}
			return false;
		}

		public static T OneOrDefault< T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IList<T> list = source as IList<T>;
			if (list != null)
			{
				if (list.Count != 1)
				{
					return default(T);
				}
				return list[0];
			}
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					return enumerator.MoveNext() ? default(T) : current;
				}
				return default(T);
			}
		}

		public static TSource OneOrDefault< TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			using (IEnumerator<TSource> enumerator = source.Where(predicate).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					TSource current = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						return current;
					}
				}
				return default(TSource);
			}
		}

		[Obsolete("建议使用命名更优雅的 OneOrDefault。这个命名很长的方法的存在，只是为了通过代码提示告诉你 SingleOrDefault 可能甩异常，从而避免你忘了处理。", false)]
		public static TSource SingleOrDefaultWithoutException< TSource>(this IEnumerable<TSource> source)
		{
			return source.OneOrDefault();
		}

		[Obsolete("建议使用命名更优雅的 OneOrDefault。这个命名很长的方法的存在，只是为了通过代码提示告诉你 SingleOrDefault 可能甩异常，从而避免你忘了处理。", false)]
		public static TSource SingleOrDefaultWithoutException< TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.OneOrDefault(predicate);
		}

		public static IEnumerable<T> SliceRow< T>(this T[,] array, int row)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.GetUpperBound(0) - array.GetLowerBound(0) <= row)
			{
				throw new ArgumentOutOfRangeException("array", "row的值比数组长度大");
			}
			for (int i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
			{
				yield return array[row, i];
			}
		}

		public static IEnumerable<T> SliceColumn< T>(this T[,] array, int column)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.GetUpperBound(1) - array.GetLowerBound(1) <= column)
			{
				throw new ArgumentOutOfRangeException("array", "column的值比数组长度大");
			}
			for (int i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
			{
				yield return array[i, column];
			}
		}
	}
}
