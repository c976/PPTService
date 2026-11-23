using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Encryption
{
	
	
	internal class FileContentParserManager : IFileContentParserManager
	{
		internal static IList<string> ExceptFile = new List<string> { "ContentParser", "Document", "SaveInfoMetadataFile" };

		private readonly List<IFileContentParser> _supportedFileContentParsers = new List<IFileContentParser>();

		public string FilePath => "ContentParser";

		public IFileContentParser ContentSaveParser { get; private set; } = new EmptyFileContentParser();

		public IFileContentParser ContentOpenParser { get; private set; } = new EmptyFileContentParser();

		internal static bool IsExceptFile(string relativePath)
		{
			return ExceptFile.Any(relativePath.Contains);
		}

		public FileContentParserManager()
		{
			LoadFileContentParser(new AesFileContentParser());
			LoadFileContentParser(new Base64FileContentParser());
		}

		public void SetContentSaveParser(IFileContentParser fileContentParser)
		{
			IFileContentParser fileContentParser2 = fileContentParser;
			if (fileContentParser2 == null)
			{
				throw new ArgumentNullException("fileContentParser");
			}
			ContentSaveParser = fileContentParser2;
			if (_supportedFileContentParsers.All( (IFileContentParser c) => c.Name != fileContentParser.Name))
			{
				_supportedFileContentParsers.Add(fileContentParser);
			}
		}

		public void SetContentOpenParser(FileContentParserSaveInfo parserSaveInfo)
		{
			if (string.IsNullOrWhiteSpace(parserSaveInfo.ParserName))
			{
				ContentOpenParser = new EmptyFileContentParser();
				return;
			}
			IFileContentParser fileContentParser = _supportedFileContentParsers.Find((IFileContentParser c) => c.Name == parserSaveInfo.ParserName);
			if (fileContentParser == null)
			{
				throw new NotSupportedFileContentParserException(parserSaveInfo.ParserName);
			}
			fileContentParser.LoadParserInfo(parserSaveInfo);
			ContentOpenParser = fileContentParser;
		}

		public void LoadFileContentParsers(IList<IFileContentParser> fileContentParsers)
		{
			if (fileContentParsers == null)
			{
				throw new ArgumentNullException("fileContentParsers");
			}
			foreach (IFileContentParser fileContentParser in fileContentParsers)
			{
				LoadFileContentParser(fileContentParser);
			}
		}

		public void LoadFileContentParser(IFileContentParser fileContentParser)
		{
			if (fileContentParser == null)
			{
				throw new ArgumentNullException("fileContentParser");
			}
			if (!_supportedFileContentParsers.Any( (IFileContentParser c) => c.Name == fileContentParser.Name))
			{
				_supportedFileContentParsers.Add(fileContentParser);
			}
		}
	}
}
