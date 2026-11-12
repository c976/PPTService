using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	internal class BoardInjectUndoRedoInsertFilter : IBoardUndoRedoInsertFilter
	{
		private Board Board { get; }

		public void FilterInsert(IOperation operation)
		{
			InjectBoard(operation);
		}

		public BoardInjectUndoRedoInsertFilter(Board board)
		{
			Board = board;
		}

		private void InjectBoard(IOperation operation)
		{
			IBoardOperation boardOperation = operation as IBoardOperation;
			if (boardOperation != null)
			{
				boardOperation.Board = Board;
			}
			IInjectBoardOperation injectBoardOperation = operation as IInjectBoardOperation;
			if (injectBoardOperation != null)
			{
				injectBoardOperation.Board = Board;
			}
			CombinationOperation combinationOperation = operation as CombinationOperation;
			if (combinationOperation == null)
			{
				return;
			}
			foreach (IOperation operation2 in combinationOperation.Operations)
			{
				InjectBoard(operation2);
			}
		}
	}
}
