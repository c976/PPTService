using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class NotSupportedFileContentParserException : StorageException
	{
		public string ParserName { get; }

		public override string Message => Info;

		public NotSupportedFileContentParserException(string parserName)
		{
			ParserName = parserName;
			Info = "Not Support FileContentParser : " + parserName;
		}
	}
}
