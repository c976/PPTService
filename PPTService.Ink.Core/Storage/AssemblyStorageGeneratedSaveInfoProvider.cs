using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage.AdvancedSaveInfos;
using PPTService.Ink.Core.Storage.Encryption;
using PPTService.Ink.Core.Storage.Standard;

namespace PPTService.Ink.Core.Storage
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
