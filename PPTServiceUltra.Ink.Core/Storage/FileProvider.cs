using System.IO;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Encryption;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class FileProvider
	{
		internal IFileContentParser FileContentSaveParser => ContentParserManager.ContentSaveParser;

		internal IFileContentParser FileContentOpenParser => ContentParserManager.ContentOpenParser;

		internal IFileContentParserManager ContentParserManager { get; }

		internal FileProvider(IFileContentParserManager contentParserManager)
		{
			ContentParserManager = contentParserManager;
		}

		internal FileItem BuildFile(StorableNode node, ISaveInfoNodeParser parser, string buildPath, string relativePath, CompoundDocumentLogger logger)
		{
			using (logger.Section(relativePath))
			{
				if (!Directory.Exists(buildPath))
				{
					Directory.CreateDirectory(buildPath);
				}
				string content = parser.DeparseValue(node);
				return FileContentSaveParser.SaveContenToFile(content, buildPath, relativePath + ".xml");
			}
		}

		internal FileItem BuildFile(SaveInfo saveInfo, ISaveInfoParser parser, string buildPath, string relativePath, CompoundDocumentLogger logger)
		{
			using (logger.Section(relativePath))
			{
				if (!Directory.Exists(buildPath))
				{
					Directory.CreateDirectory(buildPath);
				}
				string content = parser.Deparse(saveInfo);
				return FileContentSaveParser.SaveContenToFile(content, buildPath, relativePath + ".xml");
			}
		}

		internal string ReadContentFromFile(FileItem file)
		{
			return FileContentOpenParser.ReadContentFromFile(file);
		}
	}
}
