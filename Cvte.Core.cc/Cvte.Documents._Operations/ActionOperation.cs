using System;

namespace Cvte.Documents._Operations
{
	public class ActionOperation : IOperation
	{
		private readonly Action _undo;

		private readonly Action _redo;

		public ActionOperation(Action undo, Action redo)
		{
			_undo = undo;
			_redo = redo;
		}

		public void Undo()
		{
			_undo?.Invoke();
		}

		public void Redo()
		{
			_redo?.Invoke();
		}
	}
}
