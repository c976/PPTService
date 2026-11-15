using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Documents
{
	public interface IUndoRedoTransaction : IDisposable
	{
		
		void Insert(IOperation operation);

		void Commit();

		void Rollback();
	}
}
