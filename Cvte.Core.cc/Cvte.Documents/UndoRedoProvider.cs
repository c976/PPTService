using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cvte.Documents
{
	public class UndoRedoProvider : MarshalByRefObject, IUndoRedoProvider
	{
		[CompilerGenerated]
		private EventHandler<OperationEventArgs> m_UndoExecuting;

		[CompilerGenerated]
		private EventHandler<OperationEventArgs> m_RedoExecuting;

		[CompilerGenerated]
		private EventHandler<OperationEventArgs> m_UndoExecuted;

		[CompilerGenerated]
		private EventHandler<OperationEventArgs> m_RedoExecuted;

		[CompilerGenerated]
		private EventHandler m_UndoRedoEnableChanged;

		private bool _isUndoRedoing;

		private readonly UndoRedoStack _currentUndoRedoStack = new UndoRedoStack();

		private bool _canUndoSnapshot;

		private bool _canRedoSnapshot;

		private UndoRedoTransaction _currenTransaction;

		public int CurrentState => _currentUndoRedoStack.UndoStack.ToList().Aggregate(0, (int previousValue, IOperation e) => previousValue ^ e.GetHashCode());

		public bool CanUndo => _currentUndoRedoStack.UndoStack.Count != 0;

		public bool CanRedo => _currentUndoRedoStack.RedoStack.Count != 0;

		public event EventHandler<OperationEventArgs> UndoExecuting
		{
			[CompilerGenerated]
			add
			{
				EventHandler<OperationEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_UndoExecuting;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OperationEventArgs> eventHandler2 = default(EventHandler<OperationEventArgs>);
				EventHandler<OperationEventArgs> value2 = (EventHandler<OperationEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_UndoExecuting, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OperationEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<OperationEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_UndoExecuting;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OperationEventArgs> eventHandler2 = default(EventHandler<OperationEventArgs>);
				EventHandler<OperationEventArgs> value2 = (EventHandler<OperationEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_UndoExecuting, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OperationEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		public event EventHandler<OperationEventArgs> RedoExecuting
		{
			[CompilerGenerated]
			add
			{
				EventHandler<OperationEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_RedoExecuting;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OperationEventArgs> eventHandler2 = default(EventHandler<OperationEventArgs>);
				EventHandler<OperationEventArgs> value2 = (EventHandler<OperationEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_RedoExecuting, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OperationEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<OperationEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_RedoExecuting;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OperationEventArgs> eventHandler2 = default(EventHandler<OperationEventArgs>);
				EventHandler<OperationEventArgs> value2 = (EventHandler<OperationEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_RedoExecuting, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OperationEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		public event EventHandler<OperationEventArgs> UndoExecuted
		{
			[CompilerGenerated]
			add
			{
				EventHandler<OperationEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_UndoExecuted;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OperationEventArgs> eventHandler2 = default(EventHandler<OperationEventArgs>);
				EventHandler<OperationEventArgs> value2 = (EventHandler<OperationEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_UndoExecuted, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OperationEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<OperationEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_UndoExecuted;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OperationEventArgs> eventHandler2 = default(EventHandler<OperationEventArgs>);
				EventHandler<OperationEventArgs> value2 = (EventHandler<OperationEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_UndoExecuted, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OperationEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		public event EventHandler<OperationEventArgs> RedoExecuted
		{
			[CompilerGenerated]
			add
			{
				EventHandler<OperationEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_RedoExecuted;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OperationEventArgs> eventHandler2 = default(EventHandler<OperationEventArgs>);
				EventHandler<OperationEventArgs> value2 = (EventHandler<OperationEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_RedoExecuted, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OperationEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<OperationEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_RedoExecuted;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<OperationEventArgs> eventHandler2 = default(EventHandler<OperationEventArgs>);
				EventHandler<OperationEventArgs> value2 = (EventHandler<OperationEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_RedoExecuted, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<OperationEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		public event EventHandler UndoRedoEnableChanged
		{
			[CompilerGenerated]
			add
			{
				EventHandler eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_UndoRedoEnableChanged;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler eventHandler2 = default(EventHandler);
				EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_UndoRedoEnableChanged, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			[CompilerGenerated]
			remove
			{
				EventHandler eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_UndoRedoEnableChanged;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler eventHandler2 = default(EventHandler);
				EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_UndoRedoEnableChanged, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		public void Undo(int levels = 1)
		{
			if (0 == 0)
			{
				SnapshotForCanUndoAndCanRedo();
			}
			int num = 1;
			IOperation operation = default(IOperation);
			while (true)
			{
				if (4 == 0)
				{
					goto IL_003f;
				}
				if (num <= levels)
				{
					goto IL_00a9;
				}
				RaiseUndoRedoFeatureEnableChanged();
				break;
				IL_007e:
				num++;
				continue;
				IL_00a9:
				if (_currentUndoRedoStack.UndoStack.Count == 0)
				{
					goto IL_007e;
				}
				operation = _currentUndoRedoStack.UndoStack.Pop();
				goto IL_003f;
				IL_003f:
				if (3 == 0)
				{
					goto IL_00a9;
				}
				_isUndoRedoing = true;
				if (7 != 0)
				{
					RaiseUndoExecutingEvent(operation);
				}
				operation.Undo();
				if (0 == 0)
				{
					RaiseUndoExecutedEvent(operation);
					do
					{
						_isUndoRedoing = false;
					}
					while (false);
					_currentUndoRedoStack.RedoStack.Push(operation);
					goto IL_007e;
				}
				break;
			}
		}

		public void Redo(int levels = 1)
		{
			if (0 == 0)
			{
				SnapshotForCanUndoAndCanRedo();
			}
			int num = 1;
			IOperation operation = default(IOperation);
			while (true)
			{
				if (4 == 0)
				{
					goto IL_003f;
				}
				if (num <= levels)
				{
					goto IL_00a9;
				}
				RaiseUndoRedoFeatureEnableChanged();
				break;
				IL_007e:
				num++;
				continue;
				IL_00a9:
				if (_currentUndoRedoStack.RedoStack.Count == 0)
				{
					goto IL_007e;
				}
				operation = _currentUndoRedoStack.RedoStack.Pop();
				goto IL_003f;
				IL_003f:
				if (3 == 0)
				{
					goto IL_00a9;
				}
				_isUndoRedoing = true;
				if (7 != 0)
				{
					RaiseRedoExecutingEvent(operation);
				}
				operation.Redo();
				if (0 == 0)
				{
					RaiseRedoExecutedEvent(operation);
					do
					{
						_isUndoRedoing = false;
					}
					while (false);
					_currentUndoRedoStack.UndoStack.Push(operation);
					goto IL_007e;
				}
				break;
			}
		}

		public void Insert(IOperation operation)
		{
			if (operation == null)
			{
				if (0 == 0)
				{
					throw new ArgumentNullException("operation");
				}
				goto IL_00a4;
			}
			if (false)
			{
				goto IL_005b;
			}
			if (_isUndoRedoing)
			{
				return;
			}
			goto IL_008e;
			IL_0097:
			_currenTransaction.Insert(operation);
			return;
			IL_008e:
			if (_currenTransaction != null && !(operation is TransactionOperation))
			{
				goto IL_0097;
			}
			goto IL_00a4;
			IL_005b:
			if (0 == 0)
			{
				if (false)
				{
					goto IL_008e;
				}
				_currentUndoRedoStack.RedoStack.Clear();
				RaiseUndoRedoFeatureEnableChanged();
				return;
			}
			goto IL_0097;
			IL_00a4:
			SnapshotForCanUndoAndCanRedo();
			_currentUndoRedoStack.UndoStack.Push(operation);
			goto IL_005b;
		}

		public void ClearAll()
		{
			if (8 == 0 || 5 == 0)
			{
				return;
			}
			if (3u != 0)
			{
				SnapshotForCanUndoAndCanRedo();
			}
			_currentUndoRedoStack.RedoStack.Clear();
			if (2 != 0)
			{
				Stack<IOperation> undoStack = _currentUndoRedoStack.UndoStack;
				if (5u != 0)
				{
					undoStack.Clear();
				}
			}
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
			if (_canRedoSnapshot == CanRedo && _canUndoSnapshot == CanUndo)
			{
				return;
			}
			EventHandler eventHandler = this.m_UndoRedoEnableChanged;
			if (eventHandler != null)
			{
				eventHandler(this, null);
				if (0 == 0 && false)
				{
				}
			}
		}

		private void SnapshotForCanUndoAndCanRedo()
		{
			_canUndoSnapshot = CanUndo;
			_canRedoSnapshot = CanRedo;
		}

		private void RaiseUndoExecutingEvent(IOperation operatiom)
		{
			do
			{
				IL_0000:
				if (0 == 0)
				{
					EventHandler<OperationEventArgs> eventHandler = this.m_UndoExecuting;
					if (eventHandler != null)
					{
						eventHandler(this, new OperationEventArgs(operatiom));
						continue;
					}
				}
				if (false)
				{
					goto IL_0000;
				}
				if (5 != 0)
				{
					break;
				}
			}
			while (false);
		}

		private void RaiseRedoExecutingEvent(IOperation operation)
		{
			do
			{
				IL_0000:
				if (0 == 0)
				{
					EventHandler<OperationEventArgs> eventHandler = this.m_RedoExecuting;
					if (eventHandler != null)
					{
						eventHandler(this, new OperationEventArgs(operation));
						continue;
					}
				}
				if (false)
				{
					goto IL_0000;
				}
				if (5 != 0)
				{
					break;
				}
			}
			while (false);
		}

		private void RaiseUndoExecutedEvent(IOperation operation)
		{
			do
			{
				IL_0000:
				if (0 == 0)
				{
					EventHandler<OperationEventArgs> eventHandler = this.m_UndoExecuted;
					if (eventHandler != null)
					{
						eventHandler(this, new OperationEventArgs(operation));
						continue;
					}
				}
				if (false)
				{
					goto IL_0000;
				}
				if (5 != 0)
				{
					break;
				}
			}
			while (false);
		}

		private void RaiseRedoExecutedEvent(IOperation operation)
		{
			do
			{
				IL_0000:
				if (0 == 0)
				{
					EventHandler<OperationEventArgs> eventHandler = this.m_RedoExecuted;
					if (eventHandler != null)
					{
						eventHandler(this, new OperationEventArgs(operation));
						continue;
					}
				}
				if (false)
				{
					goto IL_0000;
				}
				if (5 != 0)
				{
					break;
				}
			}
			while (false);
		}
	}
}
