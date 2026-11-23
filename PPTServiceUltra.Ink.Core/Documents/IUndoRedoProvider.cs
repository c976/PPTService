using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Documents
{
	
	public interface IUndoRedoProvider
	{
		int CurrentState { get; }

		bool CanUndo { get; }

		bool CanRedo { get; }

		event EventHandler<OperationEventArgs> UndoExecuting;

		event EventHandler<OperationEventArgs> RedoExecuting;

		event EventHandler<OperationEventArgs> UndoExecuted;

		event EventHandler<OperationEventArgs> RedoExecuted;

		event EventHandler UndoRedoEnableChanged;

		void Undo(int levels = 1);

		void Redo(int levels = 1);

		void Insert(IOperation operation);

		void ClearAll();

		IUndoRedoTransaction BeginTransaction();
	}
}
