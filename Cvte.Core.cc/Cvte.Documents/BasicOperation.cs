using System;

namespace Cvte.Documents
{
	public class BasicOperation<T1> : IOperation
	{
		private readonly T1 _param1;

		private readonly Action<T1> _undoAction;

		private readonly Action<T1> _redoAction;

		public BasicOperation(Action<T1> undoAction, Action<T1> redoAction, T1 param1)
		{
			_undoAction = undoAction;
			_redoAction = redoAction;
			_param1 = param1;
		}

		public void Undo()
		{
			_undoAction(_param1);
		}

		public void Redo()
		{
			_redoAction(_param1);
		}
	}
	public class BasicOperation<T1, T2> : IOperation
	{
		private readonly T1 _param1;

		private readonly T2 _param2;

		private readonly Action<T1, T2> _undoAction;

		private readonly Action<T1, T2> _redoAction;

		public BasicOperation(Action<T1, T2> undoAction, Action<T1, T2> redoAction, T1 param1, T2 param2)
		{
			_undoAction = undoAction;
			_redoAction = redoAction;
			_param1 = param1;
			_param2 = param2;
		}

		public void Undo()
		{
			_undoAction(_param1, _param2);
		}

		public void Redo()
		{
			_redoAction(_param1, _param2);
		}
	}
	public class BasicOperation<T1, T2, T3> : IOperation
	{
		private readonly T1 _param1;

		private readonly T2 _param2;

		private readonly T3 _param3;

		private readonly Action<T1, T2, T3> _undoAction;

		private readonly Action<T1, T2, T3> _redoAction;

		public BasicOperation(Action<T1, T2, T3> undoAction, Action<T1, T2, T3> redoAction, T1 param1, T2 param2, T3 param3)
		{
			_undoAction = undoAction;
			_redoAction = redoAction;
			_param1 = param1;
			_param2 = param2;
			_param3 = param3;
		}

		public void Undo()
		{
			_undoAction(_param1, _param2, _param3);
		}

		public void Redo()
		{
			_redoAction(_param1, _param2, _param3);
		}
	}
	public class BasicOperation<T1, T2, T3, T4> : IOperation
	{
		private readonly T1 _param1;

		private readonly T2 _param2;

		private readonly T3 _param3;

		private readonly T4 _param4;

		private readonly Action<T1, T2, T3, T4> _undoAction;

		private readonly Action<T1, T2, T3, T4> _redoAction;

		public BasicOperation(Action<T1, T2, T3, T4> undoAction, Action<T1, T2, T3, T4> redoAction, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			_undoAction = undoAction;
			_redoAction = redoAction;
			_param1 = param1;
			_param2 = param2;
			_param3 = param3;
			_param4 = param4;
		}

		public void Undo()
		{
			_undoAction(_param1, _param2, _param3, _param4);
		}

		public void Redo()
		{
			_redoAction(_param1, _param2, _param3, _param4);
		}
	}
}
