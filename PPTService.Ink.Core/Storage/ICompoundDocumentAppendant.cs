using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface ICompoundDocumentAppendant
	{
		IList<ICompoundDocumentAppendantItem> GetAppendantItems();
	}
}
