using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public static class WindowAsyncExtensions
	{
		public static async Task<bool?> ShowDialogAsync([NotNull] this Window window, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			return await Dispatcher.CurrentDispatcher.InvokeAsync((Func<bool?>)window.ShowDialog, priority);
		}
	}
}
