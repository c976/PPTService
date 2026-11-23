using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Documents
{
	public interface IUndoRedoTransaction : IDisposable
	{
		
		void Insert(IOperation operation);

		void Commit();

		void Rollback();
	}
}
