using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface ISaveInfoParser
	{
		void SetReferenceManager(IReferenceManager referenceManager);

		void SetLogger(IStorageLogger logger);

		string Deparse< T>(T saveInfo) where T : SaveInfo;

		SaveInfo Parse(string text);
	}
}
