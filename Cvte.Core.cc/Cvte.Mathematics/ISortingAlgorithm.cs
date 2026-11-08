using System;
using System.Collections.Generic;

namespace Cvte.Mathematics
{
	public interface ISortingAlgorithm
	{
		void Sort<T>(IList<T> list) where T : IComparable<T>;

		void Sort<T>(IList<T> list, IComparer<T> comparer);
	}
}
