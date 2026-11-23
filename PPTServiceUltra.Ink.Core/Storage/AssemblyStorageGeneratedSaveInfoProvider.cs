using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.AdvancedSaveInfos;
using PPTServiceUltra.Ink.Core.Storage.Encryption;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal static class AssemblyStorageGeneratedSaveInfoProvider
	{
		public static void AddStorageAssemblySaveInfo(SaveInfoManager saveInfoManager)
		{
			saveInfoManager.Register<AdvancedSaveInfoMetadata>("SaveInfoMetadata").Register<MetadataFileSaveInfo>("SaveInfoMetadataFile").Register<MetadataSaveInfo>("MetadataContract")
				.Register<CompoundDocumentSaveInfo>("Document")
				.Register<ReferenceSaveInfo>("Reference")
				.Register<RelationshipSaveInfo>("Relationship")
				.Register<AesFileContentParserSaveInfo>("AesFileContentParser")
				.Register<Base64FileContentParserSaveInfo>("Base64Parser")
				.Register<FileContentParserSaveInfo>("ContentParser")
				.Register<ItemSaveInfo>("StorableTypeItem")
				.Register<AdjustSaveInfo>("Adjust")
				.Register<StandardTypeClone.CloneItemSaveInfo>("TypeCloneItem");
		}

		private static SaveInfoManager Register< TSaveInfo>(this SaveInfoManager saveInfoManager, string attributeName) where TSaveInfo : SaveInfo, new()
		{
			saveInfoManager.RegisterSaveInfoCreator(attributeName, () => new TSaveInfo());
			return saveInfoManager;
		}
	}
}
