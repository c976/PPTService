using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public interface IBoardUndoRedoInsertFilter
	{
		void FilterInsert(IOperation operation);
	}
}
