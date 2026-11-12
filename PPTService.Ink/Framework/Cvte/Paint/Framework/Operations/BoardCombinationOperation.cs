using System.Collections.Generic;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public class BoardCombinationOperation : CombinationOperation, IBoardOperationBase, IOperation
	{
		public Board Board { get; }

		public BoardCombinationOperation(Board board, IList<IOperation> operations)
			: base(operations)
		{
			Board = board;
		}

		public BoardCombinationOperation(Board board, params IOperation[] operations)
			: base(operations)
		{
			Board = board;
		}
	}
}
