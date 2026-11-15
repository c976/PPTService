using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Document
{
	
	public interface ICompoundDocumentConverter
	{
		CompoundDocument Convert(CompoundDocument inDocument);

		string GetFileContentParserType(CompoundDocument document);
	}
}
