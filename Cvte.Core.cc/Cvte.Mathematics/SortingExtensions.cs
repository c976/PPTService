using System;
using System.Collections.Generic;

namespace Cvte.Mathematics
{
	public static class SortingExtensions
	{
		public static void Sort<T>(this IList<T> list, ISortingAlgorithm algorithm) where T : IComparable<T>
		{
			if (4 != 0)
			{
				if (list == null || 7 == 0)
				{
					throw new ArgumentNullException("list");
				}
				if (algorithm == null)
				{
					goto IL_0016;
				}
				goto IL_0021;
			}
			return;
			IL_0016:
			if (0 == 0)
			{
				throw new ArgumentNullException("algorithm");
			}
			goto IL_0021;
			IL_0021:
			if (-1 != 0)
			{
				algorithm.Sort(list);
				return;
			}
			goto IL_0016;
		}

		public static void Sort<T>(this IList<T> list, IComparer<T> comparer, ISortingAlgorithm algorithm)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			algorithm.Sort(list, comparer);
		}
	}
}
