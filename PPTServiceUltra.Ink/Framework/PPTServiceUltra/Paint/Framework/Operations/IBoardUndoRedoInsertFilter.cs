using PPTServiceUltra.Ink.Core.Documents;

namespace PPTServiceUltra.Paint.Framework.Operations
{
	public interface IBoardUndoRedoInsertFilter
	{
		void FilterInsert(IOperation operation);
	}
}
