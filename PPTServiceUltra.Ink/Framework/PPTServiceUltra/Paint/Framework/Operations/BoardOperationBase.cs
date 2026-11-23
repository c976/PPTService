using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	public abstract class BoardOperationBase : NotifyOperation, IBoardOperationBase, IOperation
	{
		public Board Board { get; }

		protected BoardOperationBase(Board board)
		{
			Board = board;
		}

		protected abstract override void OnRedo();

		protected abstract override void OnUndo();
	}
}
