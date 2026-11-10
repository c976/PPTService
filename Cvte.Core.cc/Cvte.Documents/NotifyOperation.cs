using System;

namespace Cvte.Documents
{
	public abstract class NotifyOperation : IOperation, INotifyOperation
	{
		public event EventHandler RedoExcuted;

		public event EventHandler UndoExcuted;

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
			RedoExcuted?.Invoke(this, EventArgs.Empty);
		}

		private void OnUndoExcuted()
		{
			UndoExcuted?.Invoke(this, EventArgs.Empty);
		}
	}
}
