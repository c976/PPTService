using System;
using System.Windows.Threading;

namespace Cvte.Windows.Threading
{
	public static class DispatcherExtensions
	{
		public static void BeginInvoke(this Dispatcher dispatcher, Action action)
		{
			dispatcher.BeginInvoke(action);
		}

		public static void BeginInvoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
		{
			dispatcher.BeginInvoke(action, priority);
		}

		public static void Invoke(this Dispatcher dispatcher, Action action)
		{
			dispatcher.Invoke(action);
		}

		public static void Invoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
		{
			dispatcher.Invoke(action, priority);
		}

		public static void Do(this Dispatcher dispatcher, Action action)
		{
			while (true)
			{
				if (8 == 0 || 5 == 0)
				{
					return;
				}
				if (dispatcher.CheckAccess())
				{
					if (0 == 0)
					{
						if (3u != 0)
						{
							action();
						}
						return;
					}
					continue;
				}
				if (2 == 0)
				{
					return;
				}
				break;
			}
			dispatcher.BeginInvoke(action);
		}
	}
}
