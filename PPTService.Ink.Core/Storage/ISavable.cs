using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	public interface ISavable
	{
	}
	
	public interface ISavable< T> : ISavable where T : SaveInfo
	{
		T GetSaveInfo();

		void SetSaveInfo(T saveInfo);
	}
}
