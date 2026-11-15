using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	internal interface IMetadataProvider
	{
		void ResetMetadataManager();

		List<MetadataSaveInfo> GetMetadata();

		void LoadMetadata(IList<MetadataSaveInfo> metadata);
	}
}
