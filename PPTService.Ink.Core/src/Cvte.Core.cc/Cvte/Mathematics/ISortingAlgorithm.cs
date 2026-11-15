using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Mathematics
{
	
	public interface ISortingAlgorithm
	{
		void Sort< T>(IList<T> list) where T : IComparable<T>;

		void Sort< T>(IList<T> list, IComparer<T> comparer);
	}
}
