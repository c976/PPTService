using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTService.Ink.Core.Storage.Encryption
{
	
	
	internal class EmptyFileContentParser : IFileContentParser
	{
		private static EmptyFileContentParser _emptyFileContentParser;

		internal static EmptyFileContentParser Instance => _emptyFileContentParser ?? (_emptyFileContentParser = new EmptyFileContentParser());

		public string Name => "";

		public string FileExtensionName => ".xml";

		public string DeparseToFile(string buildPath, string content)
		{
			string text = Path.Combine(buildPath, Guid.NewGuid().ToString() + FileExtensionName);
			TextFileWriter.WriteAllXmlTextSafety(text, content);
			return text;
		}

		public string ParseFromFile(string fileAbsolutePath)
		{
			return File.ReadAllText(fileAbsolutePath, Encoding.UTF8);
		}

		public FileContentParserSaveInfo BuildParserSaveInfo()
		{
			return new FileContentParserSaveInfo
			{
				ParserName = Name
			};
		}

		public void LoadParserInfo(FileContentParserSaveInfo parserInfo)
		{
		}

		public void Initialize()
		{
		}
	}
}
