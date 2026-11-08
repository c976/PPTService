using System;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Documents
{
	public class OperationEventArgs : EventArgs
	{
		public IOperation Operation { get; }

		public OperationEventArgs([NotNull] IOperation operation)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			Operation = operation;
		}
	}
}
