using System;

namespace Cvte.Documents
{
	public interface IUndoRedoTransaction : IDisposable
	{
		void Insert(IOperation operation);

		void Commit();

		void Rollback();
	}
}
