using System;
using System.Windows.Threading;

namespace PPTService.Ink.Core.Windows.Threading
{
	public static class DispatcherExtensions
	{
		[Obsolete("请使用 .NET Framework 4.5 中新增的 InvokeAsync 代替。")]
		public static void BeginInvoke(this Dispatcher dispatcher, Action action)
		{
			dispatcher.BeginInvoke(action);
		}

		[Obsolete("请使用 .NET Framework 4.5 中新增的 InvokeAsync 代替。")]
		public static void BeginInvoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
		{
			dispatcher.BeginInvoke(action, priority);
		}

		[Obsolete("请使用 .NET Framework 4.5 中的 Invoke 代替。")]
		public static void Invoke(this Dispatcher dispatcher, Action action)
		{
			dispatcher.Invoke(action);
		}

		[Obsolete("请使用 .NET Framework 4.5 中的 Invoke 代替。")]
		public static void Invoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
		{
			dispatcher.Invoke(action, priority);
		}

		[Obsolete("请使用 UIDispatcher.Continue 代替，以获得异步等待和返回值的支持。")]
		public static void Do(this Dispatcher dispatcher, Action action)
		{
			if (dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				dispatcher.BeginInvoke(action);
			}
		}
	}
}
