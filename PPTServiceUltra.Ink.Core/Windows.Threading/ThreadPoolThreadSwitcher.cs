using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace PPTServiceUltra.Ink.Core.Windows.Threading
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct ThreadPoolThreadSwitcher : INotifyCompletion
	{
		public bool IsCompleted => SynchronizationContext.Current == null;

		public ThreadPoolThreadSwitcher GetAwaiter()
		{
			return this;
		}

		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			ThreadPool.QueueUserWorkItem((object _) =>
			{
				continuation();
			});
		}
	}
}
