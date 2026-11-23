using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Encryption;

namespace PPTServiceUltra.Ink.Core.Storage.CompoundDocumentBuilders
{
	
	
	internal class DocumentExtensionBuilder : DocumentBuilderBase
	{
		public DocumentExtensionBuilder(CompoundDocument document, FileProvider fileProvider, ISaveInfoParser parser, string buildPath, CompoundDocumentLogger documentLogger)
			: base(document, fileProvider, parser, buildPath, documentLogger)
		{
		}

		public void AddMetadataContract(List<MetadataSaveInfo> metadata)
		{
			if (metadata.Any())
			{
				Document.AddFile(FileProvider.BuildFile(new MetadataFileSaveInfo
				{
					Metadata = metadata
				}, Parser, BuildPath, "SaveInfoMetadataFile", DocumentLogger));
			}
		}

		public void AddFileContentParserInfo()
		{
			if (!(FileProvider.FileContentSaveParser is EmptyFileContentParser))
			{
				Document.AddFile(FileProvider.BuildFile(FileProvider.FileContentSaveParser.BuildParserSaveInfo(), Parser, BuildPath, FileProvider.ContentParserManager.FilePath, DocumentLogger));
			}
		}

		public void AddThumbnail(CompoundDocumentStorageModel model)
		{
			if (model.Thumbnail != null && File.Exists(model.Thumbnail))
			{
				string thumbnailName = model.GetThumbnailName();
				Document.AddFile(model.Thumbnail, thumbnailName);
			}
		}

		public void AddCompoundDocumentAppendant(CompoundDocumentStorageModel model)
		{
			ICompoundDocumentAppendant compoundDocumentAppendant = model as ICompoundDocumentAppendant;
			if (compoundDocumentAppendant != null)
			{
				DocumentAppendantBuilder.AppendFileItem(compoundDocumentAppendant, Document, DocumentLogger, (saveInfo, relativePath) => FileProvider.BuildFile(saveInfo, Parser, BuildPath, relativePath, DocumentLogger));
			}
		}
	}
}
