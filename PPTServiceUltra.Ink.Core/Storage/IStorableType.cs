using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface IStorableType
	{
		StorableNode Deparse();

		void Parse(StorableNode node);
	}
}
