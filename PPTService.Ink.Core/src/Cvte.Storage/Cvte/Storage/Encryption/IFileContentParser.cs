using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Encryption
{
	
	public interface IFileContentParser
	{
		string Name { get; }

		string FileExtensionName { get; }

		string DeparseToFile(string buildPath, string content);

		string ParseFromFile(string fileAbsolutePath);

		FileContentParserSaveInfo BuildParserSaveInfo();

		void LoadParserInfo(FileContentParserSaveInfo parserInfo);

		void Initialize();
	}
}
