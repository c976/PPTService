using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Documents
{
	internal class TransactionOperation : CombinationOperation
	{
		
		public TransactionOperation(IList<IOperation> operations)
			: base(operations)
		{
		}
	}
}
