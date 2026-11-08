using System;

namespace Cvte.Windows.Threading
{
	public class TaskExcecutedEventArgs : EventArgs
	{
		public object Result { get; private set; }

		public object Argument { get; private set; }

		public Exception Error { get; private set; }

		public TaskExcecutedEventArgs(object result, object argument, Exception error)
		{
			Result = result;
			Argument = argument;
			Error = error;
		}
	}
}
