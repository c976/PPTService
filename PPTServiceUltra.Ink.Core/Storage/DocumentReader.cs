using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal static class DocumentReader
	{
		internal static TModel Read< TModel>(CompoundDocument document, ParserProvider parserProvider, ReferenceProvider referenceProvider, FileProvider fileProvider, out IStorageLogger logger) where TModel : CompoundDocumentStorageModel
		{
			string version = document.Version;
			ISaveInfoParser saveInfoParser = parserProvider.GetSaveInfoParser(version);
			CompoundDocumentLogger compoundDocumentLogger = new CompoundDocumentLogger();
			saveInfoParser.SetLogger(compoundDocumentLogger);
			saveInfoParser.SetReferenceManager(referenceProvider.ReferenceManager);
			TModel val = Activator.CreateInstance<TModel>();
			fileProvider.ContentParserManager.ReadAndSetContentOpenParser(document, saveInfoParser, ".xml");
			IList<ReferenceInfo> references = referenceProvider.ReadReferences(document, saveInfoParser, fileProvider);
			referenceProvider.ReferenceManager.Load(references);
			IList<MetadataSaveInfo> metadata = ReadMetadata(document, saveInfoParser, fileProvider);
			(saveInfoParser as IMetadataProvider)?.LoadMetadata(metadata);
			PropertyInfo[] contentMembers = val.GetContentMembers();
			foreach (PropertyInfo propertyInfo in contentMembers)
			{
				string storageInfoMemberName = propertyInfo.GetStorageInfoMemberName();
				string storageInfoMemberPath = propertyInfo.GetStorageInfoMemberPath();
				string storageInfoMemberNameRegex = propertyInfo.GetStorageInfoMemberNameRegex();
				bool storageInfoMemberIsRequired = propertyInfo.GetStorageInfoMemberIsRequired();
				if (string.IsNullOrEmpty(storageInfoMemberNameRegex))
				{
					string text = Path.Combine(storageInfoMemberPath, storageInfoMemberName + ".xml");
					FileItem fileItem;
					if (document.TryGetFile(text, out fileItem))
					{
						if (!File.Exists(fileItem.AbsolutePath))
						{
							if (storageInfoMemberIsRequired)
							{
								compoundDocumentLogger.Error(new ContentFileMissingException(text));
							}
							continue;
						}
						using (compoundDocumentLogger.Section(fileItem.RelativePath))
						{
							SaveInfo saveInfo = saveInfoParser.Parse(fileProvider.ReadContentFromFile(fileItem));
							if (saveInfo != null)
							{
								propertyInfo.SetValue(val, saveInfo, null);
							}
						}
					}
					else if (storageInfoMemberIsRequired)
					{
						compoundDocumentLogger.Error(new ContentFileMissingException(text));
					}
				}
				else
				{
					if (!propertyInfo.PropertyType.IsGenericType)
					{
						continue;
					}
					Type typeFromHandle = typeof(List<>);
					Type type = propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();
					if (type == null)
					{
						continue;
					}
					object obj = Activator.CreateInstance(typeFromHandle.MakeGenericType(type));
					foreach (FileItem file in document.GetFiles(storageInfoMemberPath))
					{
						if (!File.Exists(file.AbsolutePath))
						{
							if (storageInfoMemberIsRequired)
							{
								compoundDocumentLogger.Error(new ContentFileMissingException(file.RelativePath));
							}
						}
						else
						{
							using (compoundDocumentLogger.Section(file.RelativePath))
							{
								SaveInfo value = saveInfoParser.Parse(fileProvider.ReadContentFromFile(file));
								((IList)obj).Add(value);
							}
						}
					}
					propertyInfo.SetValue(val, obj, null);
				}
			}
			ReadAppendantFile(document, fileProvider, val, saveInfoParser, compoundDocumentLogger);
			compoundDocumentLogger.Document = val.Document;
			logger = compoundDocumentLogger;
			return val;
		}

		[CanBeNull]
		internal static StorableNode ReadNode< TModel>(CompoundDocument document, ParserProvider parserProvider, ReferenceProvider referenceProvider, FileProvider fileProvider, out CompoundDocumentLogger logger) where TModel : CompoundDocumentStorageModel
		{
			string version = document.Version;
			CompoundDocumentLogger compoundDocumentLogger = (logger = new CompoundDocumentLogger());
			ISaveInfoParser saveInfoParser;
			ISaveInfoNodeParser saveInfoNodeParser;
			try
			{
				saveInfoParser = parserProvider.GetSaveInfoParser(version);
				saveInfoNodeParser = parserProvider.GetSaveInfoNodeParser(version);
			}
			catch (StorageException exception)
			{
				logger.Error(exception);
				return null;
			}
			saveInfoParser.SetLogger(compoundDocumentLogger);
			saveInfoParser.SetReferenceManager(referenceProvider.ReferenceManager);
			saveInfoNodeParser.SetLogger(compoundDocumentLogger);
			saveInfoNodeParser.SetReferenceManager(referenceProvider.ReferenceManager);
			TModel val = Activator.CreateInstance<TModel>();
			fileProvider.ContentParserManager.ReadAndSetContentOpenParser(document, saveInfoParser, ".xml");
			StorableNode storableNode = new StorableNode
			{
				InternalKey = "Internal_Root_Node"
			};
			IList<ReferenceInfo> list = referenceProvider.ReadReferences(document, saveInfoParser, fileProvider);
			referenceProvider.ReferenceManager.Load(list);
			PropertyInfo[] contentMembers = val.GetContentMembers();
			PropertyInfo documentPropertyInfo = GetDocumentPropertyInfo(val);
			PropertyInfo[] array = contentMembers;
			foreach (PropertyInfo propertyInfo in array)
			{
				string storageInfoMemberName = propertyInfo.GetStorageInfoMemberName();
				string storageInfoMemberPath = propertyInfo.GetStorageInfoMemberPath();
				if (string.IsNullOrEmpty(propertyInfo.GetStorageInfoMemberNameRegex()))
				{
					string text = Path.Combine(storageInfoMemberPath, storageInfoMemberName + ".xml");
					FileItem fileItem;
					if (document.TryGetFile(text, out fileItem))
					{
						ReadNodeFromFileItem(fileItem, compoundDocumentLogger, saveInfoNodeParser, storableNode, fileProvider);
						if (documentPropertyInfo == propertyInfo)
						{
							SaveInfo saveInfo = saveInfoParser.Parse(fileProvider.ReadContentFromFile(fileItem));
							if (saveInfo != null)
							{
								propertyInfo.SetValue(val, saveInfo, null);
							}
						}
					}
					else
					{
						compoundDocumentLogger.Error(new ContentFileMissingException(text));
					}
				}
				else
				{
					if (!propertyInfo.PropertyType.IsGenericType || propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault() == null)
					{
						continue;
					}
					foreach (FileItem file in document.GetFiles(storageInfoMemberPath))
					{
						ReadNodeFromFileItem(file, compoundDocumentLogger, saveInfoNodeParser, storableNode, fileProvider);
					}
				}
			}
			referenceProvider.LoadReferenceNode(storableNode, saveInfoNodeParser, list);
			compoundDocumentLogger.Document = val.Document;
			return storableNode;
		}

		private static PropertyInfo GetDocumentPropertyInfo< TModel>(TModel model) where TModel : CompoundDocumentStorageModel
		{
			return model.GetType().GetProperty("Document");
		}

		private static void ReadNodeFromFileItem(FileItem file, CompoundDocumentLogger documentLogger, ISaveInfoNodeParser nodeParser, StorableNode rootNode, FileProvider fileProvider)
		{
			if (!File.Exists(file.AbsolutePath))
			{
				documentLogger.Error(new ContentFileMissingException(file.RelativePath));
				return;
			}
			using (documentLogger.Section(file.RelativePath))
			{
				StorableNode storableNode = nodeParser.ParseValue(fileProvider.ReadContentFromFile(file));
				nodeParser.ParseNode(storableNode);
				storableNode.RelativePath = file.RelativePath;
				rootNode.AddChild(storableNode);
			}
		}

		private static IList<MetadataSaveInfo> ReadMetadata(CompoundDocument document, ISaveInfoParser parser, FileProvider fileProvider)
		{
			FileItem fileItem;
			if (document.TryGetFile("SaveInfoMetadataFile.xml", out fileItem))
			{
				MetadataFileSaveInfo metadataFileSaveInfo = parser.Parse(fileProvider.ReadContentFromFile(fileItem)) as MetadataFileSaveInfo;
				if (metadataFileSaveInfo != null)
				{
					return metadataFileSaveInfo.Metadata;
				}
			}
			return new List<MetadataSaveInfo>();
		}

		private static void ReadAppendantFile< TModel>(CompoundDocument document, FileProvider fileProvider, TModel model, ISaveInfoParser parser, CompoundDocumentLogger logger) where TModel : CompoundDocumentStorageModel
		{
			ICompoundDocumentAppendant compoundDocumentAppendant = model as ICompoundDocumentAppendant;
			if (compoundDocumentAppendant == null)
			{
				return;
			}
			foreach (ICompoundDocumentAppendantItem appendantItem in compoundDocumentAppendant.GetAppendantItems())
			{
				try
				{
					FileItem fileItem = document.Files.FirstOrDefault( (FileItem f) => string.Equals(f.RelativePath, appendantItem.RelativePath + ".xml", StringComparison.OrdinalIgnoreCase));
					if (fileItem != null)
					{
						SaveInfo saveInfo = parser.Parse(fileProvider.ReadContentFromFile(fileItem));
						appendantItem.SetSaveInfo(saveInfo);
					}
				}
				catch (Exception ex)
				{
					logger.Warning("ReadAppendantFile Error. " + appendantItem.RelativePath + "; " + ex.Message);
				}
			}
		}
	}
}
