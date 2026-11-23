using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Documents
{
	
	
	public class OperationEventArgs : EventArgs
	{
		public IOperation Operation { get; }

		public OperationEventArgs(IOperation operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			Operation = operation;
		}
	}
}
