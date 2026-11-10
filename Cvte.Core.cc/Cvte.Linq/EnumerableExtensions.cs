using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Linq
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>([NotNull] this IEnumerable<T> source, [NotNull] Action<T> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (T item in source)
			{
				action(item);
			}
		}

		public static void ForEach<T>([NotNull] this IEnumerable<T> source, [NotNull] Action<T, int> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			int num = 0;
			foreach (T item in source)
			{
				action(item, num++);
			}
		}

		public unsafe static int FirstIndexOf<T>([NotNull] this IEnumerable<T> source, [NotNull] Predicate<T> predicate)
		{
			void* ptr = stackalloc byte[8];
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			*(int*)ptr = -1;
			foreach (T item in source)
			{
				(*(int*)ptr)++;
				if (!predicate(item))
				{
					continue;
				}
				((int*)ptr)[1] = *(int*)ptr;
				goto IL_0062;
			}
			return -1;
			IL_0062:
			return ((int*)ptr)[1];
		}

		public static int LastIndexOf<T>([NotNull] this IEnumerable<T> source, [NotNull] Predicate<T> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			ICollection<T> obj = (source as ICollection<T>) ?? source.ToArray();
			int num = obj.Count;
			foreach (T item in obj.Reverse())
			{
				num--;
				if (predicate(item))
				{
					return num;
				}
			}
			return -1;
		}
	}
}
