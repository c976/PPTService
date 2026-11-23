using System.Collections.Generic;

namespace LinqToVisualTree
{
	public interface ILinqTree<SA_T0>
	{
		SA_T0 Parent { get; }

		IEnumerable<SA_T0> Children();
	}
}
