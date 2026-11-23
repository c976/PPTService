using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
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
