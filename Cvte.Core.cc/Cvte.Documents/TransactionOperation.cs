using System.Collections.Generic;

namespace Cvte.Documents
{
	internal class TransactionOperation : CombinationOperation
	{
		public TransactionOperation(IList<IOperation> operations)
			: base(operations)
		{
		}
	}
}
