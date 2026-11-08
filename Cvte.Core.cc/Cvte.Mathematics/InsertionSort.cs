using System;
using System.Collections.Generic;

namespace Cvte.Mathematics
{
	public class InsertionSort : SortingAlgorithm
	{
		public unsafe override void Sort<T>(IList<T> list, IComparer<T> comparer)
		{
			while (true)
			{
				void* ptr = stackalloc byte[12];
				if (list == null)
				{
					throw new ArgumentNullException("list");
				}
				if (comparer == null)
				{
					break;
				}
				*(int*)ptr = list.Count;
				((int*)ptr)[1] = 1;
				while (true)
				{
					if (((int*)ptr)[1] >= *(int*)ptr)
					{
						return;
					}
					T val = list[((int*)ptr)[1]];
					((int*)ptr)[2] = ((int*)ptr)[1] - 1;
					while (((int*)ptr)[2] >= 0)
					{
						if (6 == 0)
						{
							goto end_IL_00b8;
						}
						if (comparer.Compare(list[((int*)ptr)[2]], val) <= 0)
						{
							break;
						}
						list[((int*)ptr)[2] + 1] = list[((int*)ptr)[2]];
						((int*)ptr)[2]--;
					}
					list[((int*)ptr)[2] + 1] = val;
					((int*)ptr)[1]++;
					continue;
					end_IL_00b8:
					break;
				}
			}
			throw new ArgumentNullException("comparer");
		}
	}
}
