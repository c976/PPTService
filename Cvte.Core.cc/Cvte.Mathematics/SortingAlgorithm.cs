using System;
using System.Collections.Generic;

namespace Cvte.Mathematics
{
	public abstract class SortingAlgorithm : ISortingAlgorithm
	{
		public void Sort<T>(IList<T> list) where T : IComparable<T>
		{
			Sort(list, Comparer<T>.Default);
		}

		public abstract void Sort<T>(IList<T> list, IComparer<T> comparer);
	}
}
