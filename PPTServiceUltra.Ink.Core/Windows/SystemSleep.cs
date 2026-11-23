using System;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public static class SystemSleep
	{
		[Flags]
		private enum ExecutionState : uint
		{
			SystemRequired = 1u,
			DisplayRequired = 2u,
			[Obsolete("This value is not supported.")]
			UserPresent = 4u,
			AwaymodeRequired = 0x40u,
			Continuous = 0x80000000u
		}

		public static void PreventForCurrentThread(bool isUI = true)
		{
			SetThreadExecutionState(isUI ? (ExecutionState.SystemRequired | ExecutionState.DisplayRequired | ExecutionState.Continuous) : (ExecutionState.SystemRequired | ExecutionState.Continuous));
		}

		public static void RestoreForCurrentThread()
		{
			SetThreadExecutionState(ExecutionState.Continuous);
		}

		public static void ResetIdle(bool isUI = true)
		{
			SetThreadExecutionState((!isUI) ? ExecutionState.SystemRequired : (ExecutionState.SystemRequired | ExecutionState.DisplayRequired));
		}

		[DllImport("kernel32")]
		private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
	}
}
