using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.AdvancedSaveInfos
{
	
	
	public abstract class AdvancedSaveInfo : SaveInfo
	{
		
		internal const string ContractName = "SaveInfoMetadata";

		[SaveInfoMember("SaveInfoMetadata")]
		public AdvancedSaveInfoMetadata Metadata { get; set; }
	}
}
