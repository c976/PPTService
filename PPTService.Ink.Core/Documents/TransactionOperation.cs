using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Documents
{
	internal class TransactionOperation : CombinationOperation
	{
		
		public TransactionOperation(IList<IOperation> operations)
			: base(operations)
		{
		}
	}
}
