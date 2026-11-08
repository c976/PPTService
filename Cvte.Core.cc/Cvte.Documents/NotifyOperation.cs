using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cvte.Documents
{
	public abstract class NotifyOperation : IOperation, INotifyOperation
	{
		[CompilerGenerated]
		private EventHandler m_RedoExcuted;

		[CompilerGenerated]
		private EventHandler m_UndoExcuted;

		public event EventHandler RedoExcuted
		{
			[CompilerGenerated]
			add
			{
				EventHandler eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_RedoExcuted;
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
				eventHandler = Interlocked.CompareExchange(ref this.m_RedoExcuted, value2, eventHandler2);
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
						eventHandler = this.m_RedoExcuted;
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
				eventHandler = Interlocked.CompareExchange(ref this.m_RedoExcuted, value2, eventHandler2);
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

		public event EventHandler UndoExcuted
		{
			[CompilerGenerated]
			add
			{
				EventHandler eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_UndoExcuted;
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
				eventHandler = Interlocked.CompareExchange(ref this.m_UndoExcuted, value2, eventHandler2);
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
						eventHandler = this.m_UndoExcuted;
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
				eventHandler = Interlocked.CompareExchange(ref this.m_UndoExcuted, value2, eventHandler2);
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

		public virtual void Redo()
		{
			OnRedoExcuted();
		}

		public virtual void Undo()
		{
			OnUndoExcuted();
		}

		private void OnRedoExcuted()
		{
			this.m_RedoExcuted?.Invoke(this, EventArgs.Empty);
		}

		private void OnUndoExcuted()
		{
			this.m_UndoExcuted?.Invoke(this, EventArgs.Empty);
		}
	}
}
