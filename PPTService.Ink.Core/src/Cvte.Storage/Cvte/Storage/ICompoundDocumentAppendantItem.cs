using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface ICompoundDocumentAppendantItem
	{
		string RelativePath { get; }

		void SetSaveInfo(SaveInfo saveInfo);

		
		SaveInfo GetSaveInfo(CompoundDocument compoundDocument);
	}
}
