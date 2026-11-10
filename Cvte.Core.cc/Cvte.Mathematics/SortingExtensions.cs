using System;
using System.Collections.Generic;

namespace Cvte.Mathematics
{
	public static class SortingExtensions
	{
		public static void Sort<T>(this IList<T> list, ISortingAlgorithm algorithm) where T : IComparable<T>
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			algorithm.Sort(list);
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
