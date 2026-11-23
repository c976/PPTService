using System;
using System.Windows;
using System.Windows.Interop;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	public static class WindowExtension
	{
		public static void SetWindowToForegroundWithAttachThreadInput([NotNull] Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			uint windowThreadProcessId = Win32.User32.GetWindowThreadProcessId(new WindowInteropHelper(window).Handle, IntPtr.Zero);
			uint windowThreadProcessId2 = Win32.User32.GetWindowThreadProcessId(Win32.User32.GetForegroundWindow(), IntPtr.Zero);
			Win32.User32.AttachThreadInput(windowThreadProcessId2, windowThreadProcessId, true);
			window.Show();
			window.Activate();
			Win32.User32.AttachThreadInput(windowThreadProcessId2, windowThreadProcessId, false);
			window.Topmost = true;
			window.Topmost = false;
		}
	}
}
