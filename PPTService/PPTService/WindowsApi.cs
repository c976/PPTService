using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace PPTService
{
	public class WindowsApi
	{
		public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

		[StructLayout(LayoutKind.Sequential)]
		public class SpDevinfoData
		{
			public int cbSize = Marshal.SizeOf(typeof(SpDevinfoData));

			public Guid classGuid = Guid.Empty;

			public int devInst;

			public int reserved;
		}

		public class Flagsandattributes
		{
			public const uint FileFlagWriteThrough = 2147483648u;

			public const uint FileFlagOverlapped = 1073741824u;

			public const uint FileFlagNoBuffering = 536870912u;

			public const uint FileFlagRandomAccess = 268435456u;

			public const uint FileFlagSequentialScan = 134217728u;

			public const uint FileFlagDeleteOnClose = 67108864u;

			public const uint FileFlagBackupSemantics = 33554432u;

			public const uint FileFlagPosixSemantics = 16777216u;

			public const uint FileFlagOpenReparsePoint = 2097152u;

			public const uint FileFlagOpenNoRecall = 1048576u;

			public const uint FileFlagFirstPipeInstance = 524288u;
		}

		public class SetWindowPosFlags
		{
			public const short SwpNomove = 2;

			public const short SwpNosize = 1;

			public const short SwpNozorder = 4;

			public const int SwpShowwindow = 64;
		}

		public class MouseFlages
		{
			public const int VkLbutton = 1;

			public const int MouseeventfLeftdown = 2;

			public const int MouseeventfLeftup = 4;

			public const int MouseeventfAbsolute = 32768;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		internal struct SpDeviceInterfaceDetailData
		{
			internal int cbSize;

			internal short devicePath;
		}

		internal struct SpDeviceInterfaceData
		{
			public int CbSize;

			public Guid InterfaceClassGuid;

			public int Flags;

			public int Reserved;
		}

		public class Creationdisposition
		{
			public const uint CreateNew = 1u;

			public const uint CreateAlways = 2u;

			public const uint OpenExisting = 3u;

			public const uint OpenAlways = 4u;

			public const uint TruncateExisting = 5u;
		}

		public class Desiredaccess
		{
			public const uint GenericRead = 2147483648u;

			public const uint GenericWrite = 1073741824u;

			public const uint GenericExecute = 536870912u;

			public const uint GenericAll = 268435456u;
		}

		public class WindowFlags
		{
			public const uint WsExLayered = 524288u;

			public const int WsExTransparent = 32;

			public const int GwlExstyle = -20;

			/// <summary>WS_EX_TOOLWINDOW：从 Alt+Tab / 任务栏中隐藏。</summary>
			public const uint WsExToolWindow = 128u;

			/// <summary>WS_EX_APPWINDOW：若存在会强制出现在 Alt+Tab，需清掉。</summary>
			public const uint WsExAppWindow = 262144u;

			/// <summary>WM_SYSCOMMAND 的关闭命令（系统菜单 / 标题栏关闭 / 任务栏）。</summary>
			public const int WmSysCommand = 274;

			public const int ScClose = 61536;

			/// <summary>WM_CLOSE：其它进程或任务管理器「结束任务」发的常规关闭。</summary>
			public const int WmClose = 16;

			/// <summary>系统注销 / 关机，此时必须放行，否则会卡住系统退出。</summary>
			public const int WmQueryEndSession = 17;

			public const int WmEndSession = 22;
		}

		[DllImport("hid.dll")]
		private static extern void HidD_GetHidGuid(ref Guid hidGuid);

		[DllImport("hid.dll")]
		private static extern bool HidD_GetAttributes(IntPtr hidDeviceObject, out HiddAttributes attributes);

		[DllImport("hid.dll")]
		private static extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, out IntPtr preparsedData);

		[DllImport("hid.dll")]
		private static extern uint HidP_GetCaps(IntPtr preparsedData, out HidpCaps capabilities);

		[DllImport("hid.dll")]
		private static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		[DllImport("setupapi.dll", SetLastError = true)]
		private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, uint enumerator, IntPtr hwndParent, Digcf flags);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, uint memberIndex, ref SpDeviceInterfaceData deviceInterfaceData);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SpDeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, SpDevinfoData deviceInfoData);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr CreateFile(string fileName, uint desiredAccess, uint shareMode, uint securityAttributes, uint creationDisposition, uint flagsAndAttributes, int hTemplateFile);

		[DllImport("Kernel32.dll")]
		internal static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("Kernel32.dll")]
		internal static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

		[DllImport("kernel32.dll")]
		internal static extern bool CloseHandle(IntPtr hObject);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowRect(HandleRef hWnd, out WindowRect lpRect);

		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

		[DllImport("user32", EntryPoint = "mouse_event")]
		public static extern int Mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out MousePoint lpPoint);

		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int x, int y);

		[DllImport("user32.dll")]
		public static extern int GetKeyState(int nVirtKey);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int IsIconic(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern int WindowFromPoint(int xPoint, int yPoint);

		[DllImport("user32.Dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumChildWindows(IntPtr parentHandle, EnumWindowProc callback, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr hwnd, int msg, uint wParam, uint lParam);

		[DllImport("user32")]
		public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

		[DllImport("user32")]
		public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteDC([In] IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);

		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("User32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr window, int index, int value);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetCursorPos(out System.Windows.Point pt);

		[DllImport("User32.dll")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(MousePoint point);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr ptr);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

		[DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
		public static extern bool SystemParametersInfoSet(uint action, uint uiParam, uint vparam, uint init);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		internal static void GetDeviceGuid(ref Guid hidGuid)
		{
			HidD_GetHidGuid(ref hidGuid);
		}

		internal static IntPtr GetClassDevOfHandle(Guid hidGuid)
		{
			return SetupDiGetClassDevs(ref hidGuid, 0u, IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfDeviceinterface);
		}

		internal static bool GetEnumDeviceInterfaces(IntPtr hidInfoSet, ref Guid hidGuid, uint index, ref SpDeviceInterfaceData interfaceInfo)
		{
			return SetupDiEnumDeviceInterfaces(hidInfoSet, IntPtr.Zero, ref hidGuid, index, ref interfaceInfo);
		}

		internal static bool GetDeviceInterfaceDetail(IntPtr hidInfoSet, ref SpDeviceInterfaceData interfaceInfo, IntPtr pDetail, ref int buffsize)
		{
			return SetupDiGetDeviceInterfaceDetail(hidInfoSet, ref interfaceInfo, pDetail, buffsize, ref buffsize, null);
		}

		internal static void DestroyDeviceInfoList(IntPtr hidInfoSet)
		{
			SetupDiDestroyDeviceInfoList(hidInfoSet);
		}

		internal static IntPtr CreateDeviceFile(string device)
		{
			return CreateFile(device, 3221225472u, 0u, 0u, 3u, 1073741824u, 0);
		}

		internal static void SetPopupParameter()
		{
			SystemParametersInfoSet(28u, 0u, 0u, 0u);
		}

		public static List<IntPtr> GetChildWindows(IntPtr parent)
		{
			List<IntPtr> list = new List<IntPtr>();
			GCHandle value = GCHandle.Alloc(list);
			try
			{
				EnumWindowProc callback = EnumWindow;
				EnumChildWindows(parent, callback, GCHandle.ToIntPtr(value));
			}
			finally
			{
				if (value.IsAllocated)
				{
					value.Free();
				}
			}
			list.Add(parent);
			return list;
		}

		private static bool EnumWindow(IntPtr handle, IntPtr pointer)
		{
			List<IntPtr> obj = GCHandle.FromIntPtr(pointer).Target as List<IntPtr>;
			if (obj == null)
			{
				throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
			}
			obj.Add(handle);
			return true;
		}

		internal static void GetDeviceAttribute(IntPtr device, out HiddAttributes attributes)
		{
			HidD_GetAttributes(device, out attributes);
		}

		internal static void GetPreparseData(IntPtr device, out IntPtr preparseData)
		{
			HidD_GetPreparsedData(device, out preparseData);
		}

		internal static void GetCaps(IntPtr preparseData, out HidpCaps caps)
		{
			HidP_GetCaps(preparseData, out caps);
		}

		internal static void FreePreparseData(IntPtr preparseData)
		{
			HidD_FreePreparsedData(preparseData);
		}
	}
}
