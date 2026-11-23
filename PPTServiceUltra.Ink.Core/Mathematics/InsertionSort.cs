using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Mathematics
{
	public class InsertionSort : SortingAlgorithm
	{
		
		public override void Sort< T>(IList<T> list, IComparer<T> comparer)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			int count = list.Count;
			for (int i = 1; i < count; i++)
			{
				T val = list[i];
				int num = i - 1;
				while (num >= 0 && comparer.Compare(list[num], val) > 0)
				{
					list[num + 1] = list[num];
					num--;
				}
				list[num + 1] = val;
			}
		}
	}
}
