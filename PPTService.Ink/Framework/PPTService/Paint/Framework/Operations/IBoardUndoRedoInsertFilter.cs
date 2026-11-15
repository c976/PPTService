using PPTService.Ink.Core.Documents;

namespace PPTService.Paint.Framework.Operations
{
	public interface IBoardUndoRedoInsertFilter
	{
		void FilterInsert(IOperation operation);
	}
}
