using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	internal interface IFallbackArray
	{
		IList<StorableNode> UnknownNode { get; }
	}
}
