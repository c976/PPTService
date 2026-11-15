using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface IStorableType
	{
		StorableNode Deparse();

		void Parse(StorableNode node);
	}
}
