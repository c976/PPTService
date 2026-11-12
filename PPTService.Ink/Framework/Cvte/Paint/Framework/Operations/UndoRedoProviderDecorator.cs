using System;
using System.Runtime.CompilerServices;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public abstract class UndoRedoProviderDecorator : IUndoRedoProvider
	{
		private IUndoRedoProvider RealUndoRedoProvider { get; }

		public int CurrentState => RealUndoRedoProvider.CurrentState;

		public bool CanUndo => RealUndoRedoProvider.CanUndo;

		public bool CanRedo => RealUndoRedoProvider.CanRedo;

		public event EventHandler<OperationEventArgs> UndoExecuting;

		public event EventHandler<OperationEventArgs> RedoExecuting;

		public event EventHandler<OperationEventArgs> UndoExecuted;

		public event EventHandler<OperationEventArgs> RedoExecuted;

		public event EventHandler UndoRedoEnableChanged;

		protected UndoRedoProviderDecorator(IUndoRedoProvider undoRedoProvider)
		{
			RealUndoRedoProvider = undoRedoProvider;
			undoRedoProvider.RedoExecuted += (object sender, OperationEventArgs args) =>
			{
				RedoExecuted?.Invoke(sender, args);
			};
			undoRedoProvider.RedoExecuting += (object sender, OperationEventArgs args) =>
			{
				RedoExecuting?.Invoke(sender, args);
			};
			undoRedoProvider.UndoExecuted += (object sender, OperationEventArgs args) =>
			{
				UndoExecuted?.Invoke(sender, args);
			};
			undoRedoProvider.UndoExecuting += (object sender, OperationEventArgs args) =>
			{
				UndoExecuting?.Invoke(sender, args);
			};
			undoRedoProvider.UndoRedoEnableChanged += (object sender, EventArgs args) =>
			{
				UndoRedoEnableChanged?.Invoke(sender, args);
			};
		}

		public void Undo(int levels = 1)
		{
			RealUndoRedoProvider.Undo(levels);
		}

		public void Redo(int levels = 1)
		{
			RealUndoRedoProvider.Redo(levels);
		}

		public void Insert(IOperation operation)
		{
			RealUndoRedoProvider.Insert(operation);
		}

		public void ClearAll()
		{
			RealUndoRedoProvider.ClearAll();
		}

		public IUndoRedoTransaction BeginTransaction()
		{
			return RealUndoRedoProvider.BeginTransaction();
		}
	}
}
