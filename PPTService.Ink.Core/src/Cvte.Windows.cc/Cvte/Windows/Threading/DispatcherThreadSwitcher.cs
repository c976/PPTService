using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace PPTService.Ink.Core.Windows.Threading
{
	
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct DispatcherThreadSwitcher : INotifyCompletion
	{
		private readonly Dispatcher _dispatcher;

		public bool IsCompleted => _dispatcher.CheckAccess();

		internal DispatcherThreadSwitcher(Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		public DispatcherThreadSwitcher GetAwaiter()
		{
			return this;
		}

		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			_dispatcher.InvokeAsync(continuation);
		}
	}
}
