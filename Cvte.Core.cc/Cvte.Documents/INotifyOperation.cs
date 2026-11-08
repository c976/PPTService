using System;

namespace Cvte.Documents
{
	public interface INotifyOperation : IOperation
	{
		event EventHandler RedoExcuted;

		event EventHandler UndoExcuted;
	}
}
