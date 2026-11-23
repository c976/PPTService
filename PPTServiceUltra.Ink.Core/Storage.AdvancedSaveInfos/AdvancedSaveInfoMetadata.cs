using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.AdvancedSaveInfos
{
	
	
	[SaveInfoContract("SaveInfoMetadata")]
	public class AdvancedSaveInfoMetadata : SaveInfo
	{
		[SaveInfoMember("SupportsFallback")]
		public bool SupportsFallback { get; set; }

		[SaveInfoMember("ImportanceLevel")]
		public NodeImportanceLevel? ImportanceLevel { get; set; }

		[SaveInfoMember("NodeName")]
		public string NodeName { get; set; }
	}
}
