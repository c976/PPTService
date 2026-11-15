using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Documents
{
	
	
	public class UndoRedoProvider : MarshalByRefObject, IUndoRedoProvider
	{
		private bool _isUndoRedoing;

		private readonly UndoRedoStack _currentUndoRedoStack = new UndoRedoStack();

		private bool _canUndoSnapshot;

		private bool _canRedoSnapshot;

		
		private UndoRedoTransaction _currentTransaction;

		public int CurrentState => _currentUndoRedoStack.UndoStack.ToList().Aggregate(0,  (int previousValue, IOperation e) => previousValue ^ e.GetHashCode());

		public bool CanUndo => _currentUndoRedoStack.UndoStack.Count != 0;

		public bool CanRedo => _currentUndoRedoStack.RedoStack.Count != 0;

		protected IList<IOperation> UndoStackSnapshot => _currentUndoRedoStack.UndoStack.ToList();

		protected IList<IOperation> RedoStackSnapshot => _currentUndoRedoStack.RedoStack.ToList();

		
		
		public event EventHandler<OperationEventArgs> UndoExecuting;

		
		
		public event EventHandler<OperationEventArgs> RedoExecuting;

		
		
		public event EventHandler<OperationEventArgs> UndoExecuted;

		
		
		public event EventHandler<OperationEventArgs> RedoExecuted;

		
		
		
		public event EventHandler UndoRedoEnableChanged;

		public UndoRedoProvider()
		{
		}

		public UndoRedoProvider(uint limitedOperationCount)
		{
			if (limitedOperationCount == 0)
			{
				throw new ArgumentException("不能设置限制撤销重做步数为0步");
			}
			_currentUndoRedoStack.RedoStack.LimitedCount = limitedOperationCount;
			_currentUndoRedoStack.UndoStack.LimitedCount = limitedOperationCount;
		}

		public void Undo(int levels = 1)
		{
			SnapshotForCanUndoAndCanRedo();
			for (int i = 1; i <= levels; i++)
			{
				if (_currentUndoRedoStack.UndoStack.Count == 0)
				{
					continue;
				}
				IOperation operation = _currentUndoRedoStack.UndoStack.Pop();
				_isUndoRedoing = true;
				try
				{
					RaiseUndoExecutingEvent(operation);
					operation.Undo();
					RaiseUndoExecutedEvent(operation);
					_currentUndoRedoStack.RedoStack.Push(operation);
				}
				catch (Exception)
				{
					_currentUndoRedoStack.RedoStack.Clear();
					_currentUndoRedoStack.UndoStack.Clear();
					throw;
				}
				finally
				{
					_isUndoRedoing = false;
				}
			}
			RaiseUndoRedoFeatureEnableChanged();
		}

		public void Redo(int levels = 1)
		{
			SnapshotForCanUndoAndCanRedo();
			for (int i = 1; i <= levels; i++)
			{
				if (_currentUndoRedoStack.RedoStack.Count == 0)
				{
					continue;
				}
				IOperation operation = _currentUndoRedoStack.RedoStack.Pop();
				_isUndoRedoing = true;
				try
				{
					RaiseRedoExecutingEvent(operation);
					operation.Redo();
					RaiseRedoExecutedEvent(operation);
					_currentUndoRedoStack.UndoStack.Push(operation);
				}
				catch (Exception)
				{
					_currentUndoRedoStack.RedoStack.Clear();
					_currentUndoRedoStack.UndoStack.Clear();
					throw;
				}
				finally
				{
					_isUndoRedoing = false;
				}
			}
			RaiseUndoRedoFeatureEnableChanged();
		}

		public void Insert(IOperation operation)
		{
			OnInsert(operation);
		}

		protected virtual void OnInsert(IOperation operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			if (!_isUndoRedoing)
			{
				if (_currentTransaction != null && !(operation is TransactionOperation))
				{
					_currentTransaction.Insert(operation);
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

		public virtual IUndoRedoTransaction BeginTransaction()
		{
			_currentTransaction = new UndoRedoTransaction(this, () =>
			{
				_currentTransaction = null;
			});
			return _currentTransaction;
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

		private void RaiseUndoExecutingEvent(IOperation operation)
		{
			UndoExecuting?.Invoke(this, new OperationEventArgs(operation));
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
