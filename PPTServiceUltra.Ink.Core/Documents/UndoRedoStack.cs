using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Documents
{
	
	
	public class UndoRedoStack
	{
		public LimitedStack<IOperation> UndoStack { get; }

		public LimitedStack<IOperation> RedoStack { get; }

		public UndoRedoStack()
		{
			UndoStack = new LimitedStack<IOperation>();
			RedoStack = new LimitedStack<IOperation>();
		}
	}
}
