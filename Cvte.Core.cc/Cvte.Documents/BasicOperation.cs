using System;

namespace Cvte.Documents
{
	public class BasicOperation<SA_T0> : IOperation
	{
		private readonly SA_T0 _param1;

		private readonly Action<SA_T0> _undoAction;

		private readonly Action<SA_T0> _redoAction;

		public BasicOperation(Action<SA_T0> undoAction, Action<SA_T0> redoAction, SA_T0 param1)
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
	public class BasicOperation<SA_T0, SA_T1> : IOperation
	{
		private readonly SA_T0 _param1;

		private readonly SA_T1 _param2;

		private readonly Action<SA_T0, SA_T1> _undoAction;

		private readonly Action<SA_T0, SA_T1> _redoAction;

		public BasicOperation(Action<SA_T0, SA_T1> undoAction, Action<SA_T0, SA_T1> redoAction, SA_T0 param1, SA_T1 param2)
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
	public class BasicOperation<SA_T0, SA_T1, SA_T2> : IOperation
	{
		private readonly SA_T0 _param1;

		private readonly SA_T1 _param2;

		private readonly SA_T2 _param3;

		private readonly Action<SA_T0, SA_T1, SA_T2> _undoAction;

		private readonly Action<SA_T0, SA_T1, SA_T2> _redoAction;

		public BasicOperation(Action<SA_T0, SA_T1, SA_T2> undoAction, Action<SA_T0, SA_T1, SA_T2> redoAction, SA_T0 param1, SA_T1 param2, SA_T2 param3)
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
	public class BasicOperation<SA_T0, SA_T1, SA_T2, SA_T3> : IOperation
	{
		private readonly SA_T0 _param1;

		private readonly SA_T1 _param2;

		private readonly SA_T2 _param3;

		private readonly SA_T3 _param4;

		private readonly Action<SA_T0, SA_T1, SA_T2, SA_T3> _undoAction;

		private readonly Action<SA_T0, SA_T1, SA_T2, SA_T3> _redoAction;

		public BasicOperation(Action<SA_T0, SA_T1, SA_T2, SA_T3> undoAction, Action<SA_T0, SA_T1, SA_T2, SA_T3> redoAction, SA_T0 param1, SA_T1 param2, SA_T2 param3, SA_T3 param4)
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
			do
			{
				if (0 == 0)
				{
					_undoAction(_param1, _param2, _param3, _param4);
				}
			}
			while (false);
		}

		public void Redo()
		{
			do
			{
				if (0 == 0)
				{
					_redoAction(_param1, _param2, _param3, _param4);
				}
			}
			while (false);
		}
	}
}
