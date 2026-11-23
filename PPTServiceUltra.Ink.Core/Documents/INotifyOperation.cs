using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Documents
{
	
	public interface INotifyOperation : IOperation
	{
		event EventHandler RedoExcuted;

		event EventHandler UndoExcuted;

		event EventHandler UndoExcuting;

		event EventHandler RedoExcuting;
	}
}
