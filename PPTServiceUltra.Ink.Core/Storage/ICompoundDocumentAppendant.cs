using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface ICompoundDocumentAppendant
	{
		IList<ICompoundDocumentAppendantItem> GetAppendantItems();
	}
}
