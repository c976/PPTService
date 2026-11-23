using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class FallbackArray< T> : List<T>, IFallbackArray where T : SaveInfo
	{
		public IList<StorableNode> UnknownNode { get; } = new List<StorableNode>();

		public FallbackArray()
		{
		}

		public FallbackArray(int capacity)
			: base(capacity)
		{
		}

		public FallbackArray(IEnumerable<T> collection)
			: base(collection)
		{
		}
	}
}
