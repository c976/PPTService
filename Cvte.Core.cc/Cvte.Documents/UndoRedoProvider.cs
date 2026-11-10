using System;
using System.Linq;

namespace Cvte.Documents
{
	public class UndoRedoProvider : MarshalByRefObject, IUndoRedoProvider
	{
		private bool _isUndoRedoing;

		private readonly UndoRedoStack _currentUndoRedoStack = new UndoRedoStack();

		private bool _canUndoSnapshot;

		private bool _canRedoSnapshot;

		private UndoRedoTransaction _currenTransaction;

		public int CurrentState => _currentUndoRedoStack.UndoStack.ToList().Aggregate(0, (int previousValue, IOperation e) => previousValue ^ e.GetHashCode());

		public bool CanUndo => _currentUndoRedoStack.UndoStack.Count != 0;

		public bool CanRedo => _currentUndoRedoStack.RedoStack.Count != 0;

		public event EventHandler<OperationEventArgs> UndoExecuting;

		public event EventHandler<OperationEventArgs> RedoExecuting;

		public event EventHandler<OperationEventArgs> UndoExecuted;

		public event EventHandler<OperationEventArgs> RedoExecuted;

		public event EventHandler UndoRedoEnableChanged;

		public void Undo(int levels = 1)
		{
			SnapshotForCanUndoAndCanRedo();
			for (int i = 1; i <= levels; i++)
			{
				if (_currentUndoRedoStack.UndoStack.Count != 0)
				{
					IOperation operation = _currentUndoRedoStack.UndoStack.Pop();
					_isUndoRedoing = true;
					RaiseUndoExecutingEvent(operation);
					operation.Undo();
					RaiseUndoExecutedEvent(operation);
					_isUndoRedoing = false;
					_currentUndoRedoStack.RedoStack.Push(operation);
				}
			}
			RaiseUndoRedoFeatureEnableChanged();
		}

		public void Redo(int levels = 1)
		{
			SnapshotForCanUndoAndCanRedo();
			for (int i = 1; i <= levels; i++)
			{
				if (_currentUndoRedoStack.RedoStack.Count != 0)
				{
					IOperation operation = _currentUndoRedoStack.RedoStack.Pop();
					_isUndoRedoing = true;
					RaiseRedoExecutingEvent(operation);
					operation.Redo();
					RaiseRedoExecutedEvent(operation);
					_isUndoRedoing = false;
					_currentUndoRedoStack.UndoStack.Push(operation);
				}
			}
			RaiseUndoRedoFeatureEnableChanged();
		}

		public void Insert(IOperation operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			if (!_isUndoRedoing)
			{
				if (_currenTransaction != null && !(operation is TransactionOperation))
				{
					_currenTransaction.Insert(operation);
					return;
				}
				SnapshotForCanUndoAndCanRedo();
				_currentUndoRedoStack.UndoStack.Push(operation);
				_currentUndoRedoStack.RedoStack.Clear();
				RaiseUndoRedoFeatureEnableChanged();
			}
		}

		public void ClearAll()
		{
			SnapshotForCanUndoAndCanRedo();
			_currentUndoRedoStack.RedoStack.Clear();
			_currentUndoRedoStack.UndoStack.Clear();
			RaiseUndoRedoFeatureEnableChanged();
		}

		public IUndoRedoTransaction BeginTransaction()
		{
			_currenTransaction = new UndoRedoTransaction(this, () =>
			{
				_currenTransaction = null;
			});
			return _currenTransaction;
		}

		private void RaiseUndoRedoFeatureEnableChanged()
		{
			if (_canRedoSnapshot != CanRedo || _canUndoSnapshot != CanUndo)
			{
				UndoRedoEnableChanged?.Invoke(this, null);
			}
		}

		private void SnapshotForCanUndoAndCanRedo()
		{
			_canUndoSnapshot = CanUndo;
			_canRedoSnapshot = CanRedo;
		}

		private void RaiseUndoExecutingEvent(IOperation operatiom)
		{
			UndoExecuting?.Invoke(this, new OperationEventArgs(operatiom));
		}

		private void RaiseRedoExecutingEvent(IOperation operation)
		{
			RedoExecuting?.Invoke(this, new OperationEventArgs(operation));
		}

		private void RaiseUndoExecutedEvent(IOperation operation)
		{
			UndoExecuted?.Invoke(this, new OperationEventArgs(operation));
		}

		private void RaiseRedoExecutedEvent(IOperation operation)
		{
			RedoExecuted?.Invoke(this, new OperationEventArgs(operation));
		}
	}
}
