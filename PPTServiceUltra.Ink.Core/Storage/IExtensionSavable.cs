using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface IExtensionSavable< T> : ISavable where T : SaveInfo
	{
		void GetSaveInfo(ref T saveInfo);

		void SetSaveInfo(T saveInfo);
	}
}
