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
			IEnumerator<T> enumerator = source.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (0 == 0 && 0 == 0)
					{
						T current = enumerator.Current;
						action(current);
					}
				}
			}
			finally
			{
				while (true)
				{
					if (false || enumerator != null)
					{
						goto IL_005e;
					}
					goto IL_0067;
					IL_0067:
					if (3 != 0)
					{
						break;
					}
					goto IL_005e;
					IL_005e:
					if (false)
					{
						continue;
					}
					enumerator.Dispose();
					goto IL_0067;
				}
			}
		}

		public static void ForEach<T>([NotNull] this IEnumerable<T> source, [NotNull] Action<T, int> action)
		{
			int num = default(int);
			if (source == null)
			{
				if (4 != 0)
				{
					throw new ArgumentNullException("source");
				}
			}
			else
			{
				if (6 == 0 || action == null)
				{
					throw new ArgumentNullException("action");
				}
				num = 0;
			}
			IEnumerator<T> enumerator = source.GetEnumerator();
			try
			{
				while (0 == 0 && enumerator.MoveNext())
				{
					T current = enumerator.Current;
					action(current, num++);
				}
			}
			finally
			{
				if (enumerator != null && 0 == 0 && 0 == 0)
				{
					enumerator.Dispose();
				}
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
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (0 == 0)
					{
						T current = enumerator.Current;
						(*(int*)ptr)++;
						if (8 == 0)
						{
							goto IL_007c;
						}
						if (!predicate(current))
						{
							continue;
						}
					}
					((int*)ptr)[1] = *(int*)ptr;
					goto IL_007c;
				}
			}
			return -1;
			IL_007c:
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
