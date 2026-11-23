using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public static class ScreenKeyboardHelper
	{
		[ComImport]
		[Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
		private class UIHostNoLaunch
		{
		}

		[ComImport]
		[Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface ITipInvocation
		{
			void Toggle(IntPtr hwnd);
		}

		private const string KeyboardWindowClass = "IPTip_Main_Window";

		private const string WindowParentClass = "ApplicationFrameWindow";

		private const string WindowClass = "Windows.UI.Core.CoreWindow";

		private const string WindowCaption = "Microsoft Text Input Application";

		private static bool _isFirstStart = true;

		public static bool IsKeyboardVisible => GetIsOpenKeyboardWindow() ?? GetIsOpenLegacy();

		public static void HideScreenKeyboard()
		{
			IntPtr wParam = new IntPtr(61536);
			IntPtr intPtr = FindTipMainWindow();
			if (!(intPtr == IntPtr.Zero))
			{
				Win32.User32.PostMessage(intPtr, 274u, wParam, IntPtr.Zero);
			}
		}

		public static void StartScreenKeyboard()
		{
			if (!IsKeyboardVisible || _isFirstStart)
			{
				OpenScreenKeyboard();
			}
		}

		private static bool? GetIsOpenKeyboardWindow()
		{
			if (Win32.User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Windows.UI.Core.CoreWindow", "Microsoft Text Input Application") != IntPtr.Zero)
			{
				return false;
			}
			IntPtr intPtr = Win32.User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "ApplicationFrameWindow", null);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			if (Win32.User32.FindWindowEx(intPtr, IntPtr.Zero, "Windows.UI.Core.CoreWindow", "Microsoft Text Input Application") != IntPtr.Zero)
			{
				return true;
			}
			return null;
		}

		private static bool GetIsOpenLegacy()
		{
			IntPtr intPtr = FindTipMainWindow();
			if (intPtr == IntPtr.Zero)
			{
				return false;
			}
			Win32.WindowStyles windowStyles = (Win32.WindowStyles)(int)Win32.User32.GetWindowLongPtr(intPtr, -16);
			if (windowStyles.HasFlag(Win32.WindowStyles.WS_CLIPSIBLINGS) && windowStyles.HasFlag(Win32.WindowStyles.WS_VISIBLE) && windowStyles.HasFlag(Win32.WindowStyles.WS_POPUP))
			{
				return !windowStyles.HasFlag(Win32.WindowStyles.WS_DISABLED);
			}
			return false;
		}

		private static void OpenScreenKeyboard()
		{
			string text = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
			if (text.Contains("Program Files (x86)"))
			{
				text = text.Replace("Program Files (x86)", "Program Files");
			}
			Version version = new Version(10, 0, 14393, 0);
			bool flag = Environment.OSVersion.Version >= version;
			string text2 = Path.Combine(text, "microsoft shared\\ink\\TabTip.exe");
			if (File.Exists(text2) && !flag)
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = text2,
					UseShellExecute = true,
					CreateNoWindow = true
				});
				Thread.Sleep(50);
			}
			else
			{
				try
				{
					UIHostNoLaunch uIHostNoLaunch = new UIHostNoLaunch();
					(uIHostNoLaunch as ITipInvocation)?.Toggle(Win32.User32.GetDesktopWindow());
					Marshal.ReleaseComObject(uIHostNoLaunch);
				}
				catch (Exception)
				{
				}
			}
			_isFirstStart = false;
		}

		private static IntPtr FindTipMainWindow()
		{
			return Win32.User32.FindWindow("IPTip_Main_Window", null);
		}
	}
}
