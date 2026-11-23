using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.CompoundDocumentBuilders
{
	
	
	internal class DocumentBuilderFactory
	{
		public DocumentContentBuilder ContentBuilder { get; }

		public DocumentReferenceBuilder ReferenceBuilder { get; }

		public DocumentExtensionBuilder ExtensionBuilder { get; }

		public DocumentBuilderFactory(CompoundDocument document, FileProvider fileProvider, ISaveInfoParser parser, string buildPath, CompoundDocumentLogger documentLogger)
		{
			ContentBuilder = new DocumentContentBuilder(document, fileProvider, parser, buildPath, documentLogger);
			ReferenceBuilder = new DocumentReferenceBuilder(document, fileProvider, parser, buildPath, documentLogger);
			ExtensionBuilder = new DocumentExtensionBuilder(document, fileProvider, parser, buildPath, documentLogger);
		}
	}
}
