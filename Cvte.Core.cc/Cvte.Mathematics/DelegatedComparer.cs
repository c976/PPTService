using System;
using System.Collections.Generic;

namespace Cvte.Mathematics
{
	public class DelegatedComparer<T> : IComparer<T>
	{
		private readonly Func<T, T, int> _compare;

		public DelegatedComparer(Func<T, T, int> comparingMethod)
		{
			if (comparingMethod == null)
			{
				throw new ArgumentNullException("comparingMethod");
			}
			_compare = comparingMethod;
		}

		public int Compare(T x, T y)
		{
			return _compare(x, y);
		}
	}
}
