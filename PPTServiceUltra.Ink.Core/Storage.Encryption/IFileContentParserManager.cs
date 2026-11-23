using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Encryption
{
	
	public interface IFileContentParserManager
	{
		string FilePath { get; }

		IFileContentParser ContentSaveParser { get; }

		IFileContentParser ContentOpenParser { get; }

		void SetContentSaveParser(IFileContentParser fileContentParser);

		void SetContentOpenParser(FileContentParserSaveInfo parserSaveInfo);

		void LoadFileContentParsers(IList<IFileContentParser> fileContentParsers);

		void LoadFileContentParser(IFileContentParser fileContentParser);
	}
}
