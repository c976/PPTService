using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface ISaveInfoConverter
	{
		SaveInfo Convert(StorableNode node);

		List<SaveInfo> ConvertToSaveInfos(StorableNode node);

		void SetReferenceManager(IReferenceManager referenceManager);

		void SetSaveInfoManager(SaveInfoConverterManager saveInfoConverterManager);

		void SetLogger(IStorageLogger logger);
	}
}
