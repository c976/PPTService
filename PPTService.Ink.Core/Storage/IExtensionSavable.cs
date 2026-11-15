using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface IExtensionSavable< T> : ISavable where T : SaveInfo
	{
		void GetSaveInfo(ref T saveInfo);

		void SetSaveInfo(T saveInfo);
	}
}
