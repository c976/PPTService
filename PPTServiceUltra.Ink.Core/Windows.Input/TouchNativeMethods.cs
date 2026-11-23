using System;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	internal static class TouchNativeMethods
	{
		[Flags]
		internal enum TOUCHEVENTF
		{
			TOUCHEVENTF_MOVE = 1,
			TOUCHEVENTF_DOWN = 2,
			TOUCHEVENTF_UP = 4,
			TOUCHEVENTF_INRANGE = 8,
			TOUCHEVENTF_PRIMARY = 0x10,
			TOUCHEVENTF_NOCOALESCE = 0x20,
			TOUCHEVENTF_PALM = 0x80
		}

		[Flags]
		internal enum TOUCHINPUTMASK
		{
			TOUCHINPUTMASKF_CONTACTAREA = 4,
			TOUCHINPUTMASKF_EXTRAINFO = 2,
			TOUCHINPUTMASKF_TIMEFROMSYSTEM = 1
		}

		internal struct TOUCHINPUT
		{
			public int X;

			public int Y;

			public IntPtr Source;

			public int DwID;

			public TOUCHEVENTF DwFlags;

			public TOUCHINPUTMASK DwMask;

			public int DwTime;

			public IntPtr DwExtraInfo;

			public int CxContact;

			public int CyContact;
		}

		public const int WM_TOUCH = 576;

		public const uint TWF_WANTPALM = 2u;

		public static readonly int TouchInputSize = Marshal.SizeOf((object)default(TOUCHINPUT));

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RegisterTouchWindow(IntPtr hWnd, uint ulFlags);

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [In][Out] TOUCHINPUT[] pInputs, int cbSize);

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern void CloseTouchInputHandle(IntPtr lParam);

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsTouchWindow(IntPtr hwnd, uint pulFlags);
	}
}
