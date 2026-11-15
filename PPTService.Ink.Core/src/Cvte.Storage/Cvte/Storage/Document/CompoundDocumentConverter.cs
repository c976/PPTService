using System;
using System.IO;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage.Encryption;

namespace PPTService.Ink.Core.Storage.Document
{
	
	
	public class CompoundDocumentConverter : ICompoundDocumentConverter
	{
		private readonly ISaveInfoParser _saveInfoParser;

		private readonly IFileContentParserManager _contentParserManager;

		private readonly string _buildPath;

		private const string RelativeFileExtension = ".xml";

		private IFileContentParser FileContentSaveParser => _contentParserManager.ContentSaveParser;

		private IFileContentParser FileContentOpenParser => _contentParserManager.ContentOpenParser;

		public CompoundDocumentConverter(ISaveInfoParser saveInfoParser, string buildPath)
		{
			if (saveInfoParser == null)
			{
				throw new ArgumentNullException("saveInfoParser");
			}
			_saveInfoParser = saveInfoParser;
			_buildPath = buildPath;
			_contentParserManager = new FileContentParserManager();
		}

		public CompoundDocumentConverter(ISaveInfoParser saveInfoParser, string buildPath, IFileContentParser outFileContentParser)
		{
			if (saveInfoParser == null)
			{
				throw new ArgumentNullException("saveInfoParser");
			}
			_saveInfoParser = saveInfoParser;
			if (outFileContentParser == null)
			{
				throw new ArgumentNullException("outFileContentParser");
			}
			_buildPath = buildPath;
			_contentParserManager = new FileContentParserManager();
			_contentParserManager.SetContentSaveParser(outFileContentParser);
		}

		public CompoundDocument Convert(CompoundDocument inDocument)
		{
			_contentParserManager.ReadAndSetContentOpenParser(inDocument, _saveInfoParser, ".xml");
			CompoundDocument compoundDocument = new CompoundDocument(inDocument.Name, inDocument.Version, null);
			FileContentSaveParser.Initialize();
			foreach (FileItem file3 in inDocument.Files)
			{
				if (file3.RelativePath == _contentParserManager.FilePath)
				{
					continue;
				}
				if (IsMayEntryptedFile(file3))
				{
					string content = FileContentOpenParser.ReadContentFromFile(file3);
					FileItem file = BuildFile(content, _buildPath, file3.RelativePath);
					compoundDocument.AddFile(file);
					continue;
				}
				string text = Path.Combine(_buildPath, Guid.NewGuid().ToString() + Path.GetExtension(file3.AbsolutePath));
				if (!Directory.Exists(_buildPath))
				{
					Directory.CreateDirectory(_buildPath);
				}
				File.Copy(file3.AbsolutePath, text, true);
				FileItem file2 = new FileItem(text, file3.RelativePath);
				compoundDocument.AddFile(file2);
			}
			if (!(FileContentSaveParser is EmptyFileContentParser))
			{
				string content2 = _saveInfoParser.Deparse(FileContentSaveParser.BuildParserSaveInfo());
				compoundDocument.AddFile(BuildFile(content2, _buildPath, _contentParserManager.FilePath + ".xml"));
			}
			return compoundDocument;
		}

		public string GetFileContentParserType(CompoundDocument document)
		{
			if (string.IsNullOrWhiteSpace(_contentParserManager.FilePath))
			{
				return string.Empty;
			}
			FileItem fileItem;
			if (!document.TryGetFile(_contentParserManager.FilePath + ".xml", out fileItem))
			{
				return string.Empty;
			}
			if (fileItem == null)
			{
				return string.Empty;
			}
			FileContentParserSaveInfo fileContentParserSaveInfo = _saveInfoParser.Parse(_contentParserManager.ContentOpenParser.ReadContentFromFile(fileItem)) as FileContentParserSaveInfo;
			if (fileContentParserSaveInfo != null)
			{
				return fileContentParserSaveInfo.ParserName;
			}
			return string.Empty;
		}

		private bool IsMayEntryptedFile(FileItem fileItem)
		{
			string extension = Path.GetExtension(fileItem.AbsolutePath);
			if (!(extension == ".xml"))
			{
				return extension == FileContentOpenParser.FileExtensionName;
			}
			return true;
		}

		private FileItem BuildFile(string content, string buildPath, string relativePath)
		{
			if (!Directory.Exists(buildPath))
			{
				Directory.CreateDirectory(buildPath);
			}
			return FileContentSaveParser.SaveContenToFile(content, buildPath, relativePath);
		}
	}
}
