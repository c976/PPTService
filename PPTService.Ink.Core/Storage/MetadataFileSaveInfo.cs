using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	[SaveInfoContract("SaveInfoMetadataFile", Description = "存储元数据的描述")]
	public class MetadataFileSaveInfo : SaveInfo
	{
		[SaveInfoMember("MetadataContract")]
		public IList<MetadataSaveInfo> Metadata { get; set; }
	}
}
