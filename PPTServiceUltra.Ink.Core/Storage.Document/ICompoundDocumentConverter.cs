using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Document
{
	
	public interface ICompoundDocumentConverter
	{
		CompoundDocument Convert(CompoundDocument inDocument);

		string GetFileContentParserType(CompoundDocument document);
	}
}
