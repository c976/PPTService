using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage.CompoundDocumentBuilders;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal static class DocumentBuilder
	{
		internal static CompoundDocument Build< TModel>(TModel model, ParserProvider parserProvider, ReferenceProvider referenceProvider, FileProvider fileProvider, string buildPath, out IStorageLogger logger) where TModel : CompoundDocumentStorageModel
		{
			CompoundDocumentLogger compoundDocumentLogger = new CompoundDocumentLogger();
			ISaveInfoParser currentParser = parserProvider.SaveInfoParserManager.CurrentParser;
			currentParser.SetLogger(compoundDocumentLogger);
			currentParser.SetReferenceManager(referenceProvider.ReferenceManager);
			(currentParser as IMetadataProvider)?.ResetMetadataManager();
			referenceProvider.ReferenceManager.Reset();
			CompoundDocument compoundDocument = new CompoundDocument
			{
				Name = model.Document.Name,
				Version = parserProvider.SaveInfoParserManager.CurrentVersion
			};
			DocumentBuilderFactory documentBuilderFactory = new DocumentBuilderFactory(compoundDocument, fileProvider, currentParser, buildPath, compoundDocumentLogger);
			documentBuilderFactory.ContentBuilder.AddContentFromStorageModel(model);
			documentBuilderFactory.ReferenceBuilder.AddRelationship(referenceProvider);
			IMetadataProvider metadataProvider = currentParser as IMetadataProvider;
			if (metadataProvider != null)
			{
				List<MetadataSaveInfo> metadata = metadataProvider.GetMetadata();
				documentBuilderFactory.ExtensionBuilder.AddMetadataContract(metadata);
			}
			documentBuilderFactory.ExtensionBuilder.AddFileContentParserInfo();
			documentBuilderFactory.ExtensionBuilder.AddThumbnail(model);
			documentBuilderFactory.ExtensionBuilder.AddCompoundDocumentAppendant(model);
			logger = compoundDocumentLogger;
			return compoundDocument;
		}

		
		internal static ValueTuple<StorableNode, List<MetadataSaveInfo>> BuildNode< TModel>(TModel model, ParserProvider parserProvider, ReferenceProvider referenceProvider, out IStorageLogger logger) where TModel : CompoundDocumentStorageModel
		{
			StorableNode storableNode = new StorableNode
			{
				InternalKey = "Internal_Root_Node"
			};
			logger = new CompoundDocumentLogger();
			ISaveInfoNodeParser saveInfoNodeParser = parserProvider.SaveInfoParserManager.CurrentParser as ISaveInfoNodeParser;
			if (saveInfoNodeParser == null)
			{
				return new ValueTuple<StorableNode, List<MetadataSaveInfo>>(null, new List<MetadataSaveInfo>());
			}
			saveInfoNodeParser.SetLogger(logger);
			saveInfoNodeParser.SetReferenceManager(referenceProvider.ReferenceManager);
			saveInfoNodeParser.ResetMetadataManager();
			referenceProvider.ReferenceManager.Reset();
			PropertyInfo[] contentMembers = model.GetContentMembers();
			foreach (PropertyInfo obj in contentMembers)
			{
				string storageInfoMemberName = obj.GetStorageInfoMemberName();
				string storageInfoMemberPath = obj.GetStorageInfoMemberPath();
				object value = obj.GetValue(model, null);
				SaveInfo saveInfo = value as SaveInfo;
				if (saveInfo != null)
				{
					string relativePath = Path.Combine(storageInfoMemberPath, storageInfoMemberName);
					StorableNode storableNode2 = saveInfoNodeParser.DeparseNode(saveInfo);
					storableNode2.RelativePath = relativePath;
					storableNode.AddChild(storableNode2);
					continue;
				}
				IList list = value as IList;
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						string relativePath2 = Path.Combine(storageInfoMemberPath, string.Format(storageInfoMemberName, j));
						SaveInfo saveInfo2 = list[j] as SaveInfo;
						StorableNode storableNode3 = saveInfoNodeParser.DeparseNode(saveInfo2);
						storableNode3.RelativePath = relativePath2;
						storableNode.AddChild(storableNode3);
					}
				}
			}
			referenceProvider.LoadReferenceNode(storableNode, saveInfoNodeParser, referenceProvider.ReferenceManager.References);
			List<MetadataSaveInfo> metadata = saveInfoNodeParser.GetMetadata();
			if (logger.Errors.Any())
			{
				return new ValueTuple<StorableNode, List<MetadataSaveInfo>>(null, metadata);
			}
			return new ValueTuple<StorableNode, List<MetadataSaveInfo>>(storableNode, metadata);
		}

		internal static CompoundDocument BuildDocumentFromStorableNode< TModel>(TModel model, StorableNode node, ParserProvider parserProvider, ReferenceProvider referenceProvider, FileProvider fileProvider, string buildPath, List<MetadataSaveInfo> metadata) where TModel : CompoundDocumentStorageModel
		{
			CompoundDocumentLogger compoundDocumentLogger = new CompoundDocumentLogger();
			ISaveInfoParser currentParser = parserProvider.SaveInfoParserManager.CurrentParser;
			currentParser.SetLogger(compoundDocumentLogger);
			currentParser.SetReferenceManager(referenceProvider.ReferenceManager);
			ISaveInfoNodeParser saveInfoNodeParser = parserProvider.SaveInfoParserManager.CurrentParser as ISaveInfoNodeParser;
			if (saveInfoNodeParser == null)
			{
				return null;
			}
			saveInfoNodeParser.SetLogger(compoundDocumentLogger);
			saveInfoNodeParser.SetReferenceManager(referenceProvider.ReferenceManager);
			referenceProvider.ReferenceManager.Reset();
			CompoundDocument compoundDocument = new CompoundDocument
			{
				Name = model.Document.Name,
				Version = parserProvider.SaveInfoParserManager.CurrentVersion
			};
			DocumentBuilderFactory documentBuilderFactory = new DocumentBuilderFactory(compoundDocument, fileProvider, currentParser, buildPath, compoundDocumentLogger);
			documentBuilderFactory.ContentBuilder.AddContentFromStorableNode(node, saveInfoNodeParser);
			documentBuilderFactory.ExtensionBuilder.AddMetadataContract(metadata);
			documentBuilderFactory.ExtensionBuilder.AddFileContentParserInfo();
			documentBuilderFactory.ExtensionBuilder.AddThumbnail(model);
			documentBuilderFactory.ExtensionBuilder.AddCompoundDocumentAppendant(model);
			if (compoundDocumentLogger.Errors.Any())
			{
				return null;
			}
			return compoundDocument;
		}
	}
}
