using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows.Threading
{
	public static class UIDispatcher
	{
		public static DispatcherAsyncOperation<Dispatcher> RunNewAsync([CanBeNull] string name = null)
		{
			Action<Dispatcher, Exception> reportResult;
			DispatcherAsyncOperation<Dispatcher> result = DispatcherAsyncOperation<Dispatcher>.Create(out reportResult);
			Dispatcher originDispatcher = Dispatcher.CurrentDispatcher;
			Thread thread = new Thread(() =>
			{
				try
				{
					Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
					SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(currentDispatcher));
					reportResult(currentDispatcher, null);
				}
				catch (Exception arg)
				{
					reportResult(null, arg);
				}
				try
				{
					Dispatcher.Run();
				}
				catch (Exception ex)
				{
					Exception ex2 = ex;
					Exception ex3 = ex2;
					originDispatcher.InvokeAsync(() =>
					{
						ExceptionDispatchInfo.Capture(ex3).Throw();
					});
				}
			});
			thread.Name = name ?? "BackgroundUI";
			thread.IsBackground = true;
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			return result;
		}

		public static async Task Continue([NotNull] this Dispatcher dispatcher, [NotNull] Action action, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				await dispatcher.InvokeAsync(action, priority);
			}
		}

		public static async Task<T> Continue<T>([NotNull] this Dispatcher dispatcher, [NotNull] Func<T> func, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}
			if (dispatcher.CheckAccess())
			{
				return func();
			}
			return await dispatcher.InvokeAsync(func, priority);
		}
	}
}
