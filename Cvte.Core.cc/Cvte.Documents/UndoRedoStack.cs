using System.Collections.Generic;

namespace Cvte.Documents
{
	public class UndoRedoStack
	{
		public Stack<IOperation> UndoStack { get; private set; }

		public Stack<IOperation> RedoStack { get; private set; }

		public UndoRedoStack()
		{
			UndoStack = new Stack<IOperation>();
			RedoStack = new Stack<IOperation>();
		}
	}
}
