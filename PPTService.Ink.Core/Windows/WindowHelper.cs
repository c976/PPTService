using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows
{
	internal static class WindowHelper
	{
		[Serializable]
		internal struct WINDOWPLACEMENT
		{
			public int length;

			public int flags;

			public ShowWindowCommands showCmd;

			public System.Drawing.Point ptMinPosition;

			public System.Drawing.Point ptMaxPosition;

			public Rectangle rcNormalPosition;
		}

		internal enum ShowWindowCommands
		{
			Hide,
			Normal,
			Minimized,
			Maximized
		}

		private static int GWL_EXSTYLE = Win32.GWL_EXSTYLE;

		private static int WS_EX_NOACTIVATE = 134217728;

		public static IntPtr CurrentActiveWindowHandle => Process.GetCurrentProcess().MainWindowHandle;

		public static Rect GetWindowBounds(IntPtr hwnd)
		{
			Win32.RECT rect = default(Win32.RECT);
			if (GetWindowRect(hwnd, ref rect))
			{
				System.Windows.Point location = new System.Windows.Point(rect.Left, rect.Top);
				System.Windows.Size size = new System.Windows.Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
				return new Rect(location, size);
			}
			return Rect.Empty;
		}

		public static Rect GetWindowClientBounds(IntPtr hwnd)
		{
			Win32.RECT rect = default(Win32.RECT);
			Win32.RECT rect2 = default(Win32.RECT);
			bool num = GetWindowRect(hwnd, ref rect) & GetClientRect(hwnd, ref rect2);
			int num2 = rect2.Right - rect2.Left;
			int num3 = rect.Right - rect.Left;
			int num4 = rect2.Bottom - rect2.Top;
			int num5 = rect.Bottom - rect.Top;
			int num6 = (num3 - num2) / 2;
			int num7 = num5 - num6 - num4;
			int num8 = rect.Left + num6;
			int num9 = rect.Right - num6;
			int num10 = rect.Top + num7;
			int num11 = rect.Bottom - num6;
			if (num)
			{
				return new Rect(new System.Windows.Point(num8, num10), new System.Windows.Point(num9, num11));
			}
			return Rect.Empty;
		}

		public static void SetWindowNoActivate(Window window)
		{
			IntPtr handle = new WindowInteropHelper(window).Handle;
			if (handle == IntPtr.Zero)
			{
				window.SourceInitialized -= WindowOnSourceInitialized;
				window.SourceInitialized += WindowOnSourceInitialized;
			}
			else
			{
				SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
			}
		}

		private static void WindowOnSourceInitialized(object sender, EventArgs eventArgs)
		{
			Window window = (Window)sender;
			IntPtr handle = new WindowInteropHelper(window).Handle;
			SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
			window.SourceInitialized -= WindowOnSourceInitialized;
		}

		private static int SetWindowLong(IntPtr window, int index, int value)
		{
			return Win32.SetWindowLong(window, index, value);
		}

		private static int GetWindowLong(IntPtr window, int index)
		{
			return Win32.GetWindowLong(window, index);
		}

		private static bool GetWindowRect(IntPtr hWnd, ref Win32.RECT rect)
		{
			return Win32.GetWindowRect(hWnd, out rect);
		}

		private static bool GetClientRect(IntPtr hWnd, ref Win32.RECT rect)
		{
			return Win32.GetClientRect(hWnd, ref rect);
		}

		public static IntPtr GetHandle(this Window window)
		{
			return new WindowInteropHelper(window).Handle;
		}

		public static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
		{
			WINDOWPLACEMENT lpwndpl = default(WINDOWPLACEMENT);
			lpwndpl.length = Marshal.SizeOf((object)lpwndpl);
			GetWindowPlacement(hwnd, ref lpwndpl);
			return lpwndpl;
		}

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		public static Stub OnInitialized(this Window window, Action func)
		{
			Stub stub = new Stub();
			if (window.IsLoaded)
			{
				func();
			}
			else
			{
				EventHandler[] refer = new EventHandler[1];
				refer[0] = (object sender, EventArgs e) =>
				{
					window.ContentRendered -= refer[0];
					if (stub.IsEnabled)
					{
						func();
					}
				};
				window.ContentRendered += refer[0];
			}
			return stub;
		}
	}
}
