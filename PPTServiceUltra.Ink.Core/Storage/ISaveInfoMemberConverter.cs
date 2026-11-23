using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface ISaveInfoMemberConverter
	{
		object Convert(StorableNode node);
	}
}
