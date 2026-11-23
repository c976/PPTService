using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Encryption;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal static class FileContentParserManagerExtension
	{
		public static void ReadAndSetContentOpenParser(this IFileContentParserManager manager, CompoundDocument document, ISaveInfoParser parser, string relativeFileExtension)
		{
			if (string.IsNullOrWhiteSpace(manager.FilePath))
			{
				manager.SetContentOpenParser(new FileContentParserSaveInfo());
				return;
			}
			FileItem fileItem;
			if (!document.TryGetFile(manager.FilePath + relativeFileExtension, out fileItem))
			{
				manager.SetContentOpenParser(new FileContentParserSaveInfo());
				return;
			}
			if (fileItem == null)
			{
				manager.SetContentOpenParser(new FileContentParserSaveInfo());
				return;
			}
			FileContentParserSaveInfo fileContentParserSaveInfo = parser.Parse(manager.ContentOpenParser.ReadContentFromFile(fileItem)) as FileContentParserSaveInfo;
			if (fileContentParserSaveInfo == null)
			{
				manager.SetContentOpenParser(new FileContentParserSaveInfo());
			}
			else
			{
				manager.SetContentOpenParser(fileContentParserSaveInfo);
			}
		}

		public static string ReadContentFromFile(this IFileContentParser parser, FileItem file)
		{
			if (!FileContentParserManager.IsExceptFile(file.RelativePath))
			{
				return parser.ParseFromFile(file.AbsolutePath);
			}
			return EmptyFileContentParser.Instance.ParseFromFile(file.AbsolutePath);
		}

		public static FileItem SaveContenToFile(this IFileContentParser parser, string content, string buildPath, string relativePath)
		{
			if (FileContentParserManager.IsExceptFile(relativePath))
			{
				return new FileItem(EmptyFileContentParser.Instance.DeparseToFile(buildPath, content), relativePath);
			}
			return new FileItem(parser.DeparseToFile(buildPath, content), relativePath);
		}
	}
}
