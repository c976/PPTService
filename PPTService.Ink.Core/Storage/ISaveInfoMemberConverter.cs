using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface ISaveInfoMemberConverter
	{
		object Convert(StorableNode node);
	}
}
