using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.CompoundDocumentBuilders
{
	
	
	internal abstract class DocumentBuilderBase
	{
		protected CompoundDocument Document { get; }

		protected FileProvider FileProvider { get; }

		protected ISaveInfoParser Parser { get; }

		protected string BuildPath { get; }

		protected CompoundDocumentLogger DocumentLogger { get; }

		protected DocumentBuilderBase(CompoundDocument document, FileProvider fileProvider, ISaveInfoParser parser, string buildPath, CompoundDocumentLogger documentLogger)
		{
			Document = document;
			FileProvider = fileProvider;
			Parser = parser;
			BuildPath = buildPath;
			DocumentLogger = documentLogger;
		}
	}
}
