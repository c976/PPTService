using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
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
