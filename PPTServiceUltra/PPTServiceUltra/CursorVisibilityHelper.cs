using System;
using System.Runtime.InteropServices;

namespace PPTServiceUltra
{
	/// <summary>
	/// PowerPoint 放映自带「长时间不操作隐藏鼠标指针」的逻辑；批注窗口附着到
	/// 放映窗口之上后，指针移到工具栏 / 设置界面时 PowerPoint 收不到移动消息，
	/// 就会把指针藏起来，用户看不到自己点在哪里。
	/// 放映期间由 300ms 定时器调用本类：只在系统当前确实处于隐藏状态时补一次
	/// ShowCursor(true)，不做计数累加、不改 PowerPoint 的全局设置；放映结束后
	/// 不再调用，PPT 原有的隐藏逻辑照常生效。
	/// </summary>
	internal static class CursorVisibilityHelper
	{
		private const int CursorShowing = 0x0001;

		[StructLayout(LayoutKind.Sequential)]
		private struct CursorInfo
		{
			public int cbSize;
			public int flags;
			public IntPtr hCursor;
			public MousePoint ptScreenPos;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetCursorInfo(ref CursorInfo pci);

		[DllImport("user32.dll")]
		private static extern int ShowCursor(bool bShow);

		public static void EnsureVisible()
		{
			CursorInfo info = new CursorInfo();
			info.cbSize = Marshal.SizeOf(typeof(CursorInfo));
			if (!GetCursorInfo(ref info))
			{
				return;
			}
			if ((info.flags & CursorShowing) == 0)
			{
				ShowCursor(true);
			}
		}
	}
}
