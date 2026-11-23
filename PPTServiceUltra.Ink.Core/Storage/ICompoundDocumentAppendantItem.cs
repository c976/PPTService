using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface ICompoundDocumentAppendantItem
	{
		string RelativePath { get; }

		void SetSaveInfo(SaveInfo saveInfo);

		
		SaveInfo GetSaveInfo(CompoundDocument compoundDocument);
	}
}
