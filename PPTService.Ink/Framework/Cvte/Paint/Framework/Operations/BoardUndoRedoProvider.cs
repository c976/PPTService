using System.Collections.Generic;
using Cvte.ComponentModel;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public class BoardUndoRedoProvider : UndoRedoProvider
	{
		public List<IBoardUndoRedoInsertFilter> BoardUndoRedoInsertFilterList { get; }

		public Board Board { get; }

		public BoardUndoRedoProvider(Board board)
			: this(board, uint.MaxValue)
		{
		}

		public BoardUndoRedoProvider(Board board, uint limitedOperationCount)
			: base(limitedOperationCount)
		{
			Board = board;
			BoardUndoRedoInsertFilterList = new List<IBoardUndoRedoInsertFilter>
			{
				new BoardInjectUndoRedoInsertFilter(board)
			};
			if (DebuggingProperties.IsDebug)
			{
				BoardUndoRedoInsertFilterList.Add(new NormalizeOperationsUndoRedoInsertFilter());
			}
		}

		protected override void OnInsert(IOperation operation)
		{
			foreach (IBoardUndoRedoInsertFilter boardUndoRedoInsertFilter in BoardUndoRedoInsertFilterList)
			{
				boardUndoRedoInsertFilter.FilterInsert(operation);
			}
			base.OnInsert(operation);
		}
	}
}
