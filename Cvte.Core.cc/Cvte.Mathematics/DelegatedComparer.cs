using System;
using System.Collections.Generic;

namespace Cvte.Mathematics
{
	public class DelegatedComparer<SA_T0> : IComparer<SA_T0>
	{
		private readonly Func<SA_T0, SA_T0, int> _compare;

		public DelegatedComparer(Func<SA_T0, SA_T0, int> comparingMethod)
		{
			if (comparingMethod == null)
			{
				throw new ArgumentNullException("comparingMethod");
			}
			_compare = comparingMethod;
		}

		public int Compare(SA_T0 x, SA_T0 y)
		{
			return _compare(x, y);
		}
	}
}
