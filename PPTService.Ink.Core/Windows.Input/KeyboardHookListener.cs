using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows.Input
{
	public static class KeyboardHookListener
	{
		private static class InterceptKeys
		{
			public const int WH_KEYBOARD_LL = 13;

			public const int WH_KEYBOARD = 2;

			public static IntPtr SetHook(Win32.User32.LowLevelKeyboardProc proc, int hookType, uint threadId)
			{
				using (Process process = Process.GetCurrentProcess())
				{
					using (ProcessModule processModule = process.MainModule)
					{
						return Win32.User32.SetWindowsHookEx(hookType, proc, Win32.Kernel32.GetModuleHandle(processModule?.ModuleName), threadId);
					}
				}
			}
		}

		private static Win32.User32.LowLevelKeyboardProc _proc;

		private static IntPtr _threadHookId = IntPtr.Zero;

		private static IntPtr _globalHookId = IntPtr.Zero;

		public static void HookKeyboard()
		{
			if (_proc == null)
			{
				_proc = HookCallback;
				_threadHookId = InterceptKeys.SetHook(_proc, 2, Win32.Kernel32.GetCurrentThreadId());
				_globalHookId = InterceptKeys.SetHook(_proc, 13, 0u);
			}
		}

		public static void UnhookKeyboard()
		{
			if (_threadHookId != IntPtr.Zero)
			{
				Win32.User32.UnhookWindowsHookEx(_threadHookId);
			}
			if (_globalHookId != IntPtr.Zero)
			{
				Win32.User32.UnhookWindowsHookEx(_globalHookId);
			}
			_proc = null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			return IntPtr.Zero;
		}
	}
}
