using System;
using System.ComponentModel;

namespace Cvte.Paint.Inks
{
	public static class EraseEventHelper
	{
		[Obsolete("耦合EasiNet，在解决完冲突删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsReady => IsReadyInvoker();

		[Obsolete("耦合EasiNet，在解决完冲突删除")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Func<bool> IsReadyInvoker { private get; set; }

		public static event EventHandler<EraseStartEventArgs> EraseStart;

		public static event EventHandler<EraseRunningEventArgs> EraseRunning;

		public static event EventHandler<EraseCompleteEventArgs> EraseComplete;

		public static event EventHandler<SelectionEraseEventArgs> SelectionEraseInvoked;

		internal static void OnEraseStart(object sender, EraseStartEventArgs args)
		{
			EraseStart?.Invoke(sender, args);
		}

		internal static void OnEraseRunning(object sender, EraseRunningEventArgs args)
		{
			EraseRunning?.Invoke(sender, args);
		}

		internal static void OnEraseComplete(object sender, EraseCompleteEventArgs args)
		{
			EraseComplete?.Invoke(sender, args);
		}

		internal static void OnSelectionEraseInvoked(object sender, SelectionEraseEventArgs args)
		{
			SelectionEraseInvoked?.Invoke(sender, args);
		}
	}
}
