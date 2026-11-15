using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public abstract class StorableNodeParserBase<T> : NodeParserBase<T> where T :  IStorableType
	{
	}
}
