using System;
using System.Collections.Generic;

namespace Cvte.Mathematics
{
	public class InsertionSort : SortingAlgorithm
	{
		public unsafe override void Sort<T>(IList<T> list, IComparer<T> comparer)
		{
			void* ptr = stackalloc byte[12];
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			*(int*)ptr = list.Count;
			((int*)ptr)[1] = 1;
			while (((int*)ptr)[1] < *(int*)ptr)
			{
				T val = list[((int*)ptr)[1]];
				((int*)ptr)[2] = ((int*)ptr)[1] - 1;
				while (((int*)ptr)[2] >= 0 && comparer.Compare(list[((int*)ptr)[2]], val) > 0)
				{
					list[((int*)ptr)[2] + 1] = list[((int*)ptr)[2]];
					((int*)ptr)[2]--;
				}
				list[((int*)ptr)[2] + 1] = val;
				((int*)ptr)[1]++;
			}
		}
	}
}
