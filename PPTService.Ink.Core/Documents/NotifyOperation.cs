using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Documents
{
	
	
	public abstract class NotifyOperation : INotifyOperation, IOperation
	{
		public event EventHandler UndoExcuting;

		public event EventHandler RedoExcuting;

		public event EventHandler RedoExcuted;

		public event EventHandler UndoExcuted;

		[Obsolete("禁止重写此方法，防止事件没有通知，请使用OnRedo代替")]
		public virtual void Redo()
		{
			OnRedoExcuting();
			OnRedo();
			OnRedoExcuted();
		}

		[Obsolete("禁止重写此方法，防止事件没有通知，请使用OnUndo代替")]
		public virtual void Undo()
		{
			OnUndoExcuting();
			OnUndo();
			OnUndoExcuted();
		}

		protected virtual void OnRedo()
		{
		}

		protected virtual void OnUndo()
		{
		}

		private void OnRedoExcuted()
		{
			RedoExcuted?.Invoke(this, EventArgs.Empty);
		}

		private void OnUndoExcuted()
		{
			UndoExcuted?.Invoke(this, EventArgs.Empty);
		}

		private void OnRedoExcuting()
		{
			RedoExcuting?.Invoke(this, EventArgs.Empty);
		}

		private void OnUndoExcuting()
		{
			UndoExcuting?.Invoke(this, EventArgs.Empty);
		}
	}
}
