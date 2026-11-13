using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace PPTService
{
	public static class WindowStateHelper
	{
		public static WindowRect PreWindowRect;

		private static readonly int Offset = 5;

		public static IntPtr LoadingWindowIntPtr;

		private static readonly int MaxFindWindows = 1000;

		private static readonly string RemoteShow = "remoteshow";

		private static readonly StringBuilder Sb = new StringBuilder(1024);

		public static void UpdateWindowRect(Window window)
		{
			WindowRect rect;
			WindowsApi.GetWindowRect(new HandleRef(window, PptOperator.Instance.SlideShowWindowIntPtr), out rect);
			if (Math.Abs(rect.Left - PreWindowRect.Left) >= 1 || Math.Abs(rect.Top - PreWindowRect.Top) >= 1 || Math.Abs(rect.Right - PreWindowRect.Right) >= 1 || Math.Abs(rect.Bottom - PreWindowRect.Bottom) >= 1)
			{
				SyntacticSugar.CurrentDispatcherInvoke(() =>
				{
					window.Top = rect.Top;
					window.Left = (double)rect.Left / ScreenHelper.DpiFactor;
					window.Width = (double)(rect.Right - rect.Left) / ScreenHelper.DpiFactor;
					window.Height = (double)(rect.Bottom - rect.Top) / ScreenHelper.DpiFactor;
				});
				PreWindowRect = rect;
			}
		}

		public static bool GetPptShowingState(Window window)
		{
			// 模拟放映下没有真正的 PPT 子窗口链，直接认为正在放映，避免界面被置为全透明。
			if (DebugMode.IsEnabled)
			{
				return true;
			}
			Sb.Clear();
			List<IntPtr> childWindows = WindowsApi.GetChildWindows(PptOperator.Instance.SlideShowWindowIntPtr);
			childWindows.Add(new WindowInteropHelper(window).Handle);
			childWindows.Add(LoadingWindowIntPtr);
			IntPtr intPtr = WindowsApi.WindowFromPoint(new MousePoint(window.Left, window.Top));
			IntPtr intPtr2 = WindowsApi.WindowFromPoint(new MousePoint(window.Left + window.Width - (double)Offset, window.Top + (double)Offset));
			IntPtr item = WindowsApi.WindowFromPoint(new MousePoint(window.Left + window.Width - (double)Offset, window.Height - (double)Offset));
			IntPtr item2 = WindowsApi.WindowFromPoint(new MousePoint(window.Left + window.Width / 2.0 - (double)Offset, window.Top + window.Height / 2.0 - (double)Offset));
			if (intPtr == intPtr2)
			{
				WindowsApi.GetWindowText(intPtr, Sb, Sb.Capacity);
				if (Sb.ToString().ToLower() == RemoteShow)
				{
					WindowsApi.SetWindowPos(intPtr, 0, 0, 1, 0, 0, 69);
					return true;
				}
			}
			if (!childWindows.Contains(item2) && !childWindows.Contains(intPtr) && !childWindows.Contains(intPtr2))
			{
				return childWindows.Contains(item);
			}
			return true;
		}

		public static void MakeAppAbovePpt(Window window)
		{
			IntPtr intPtr = new WindowInteropHelper(window).Handle;
			for (int i = 0; i < MaxFindWindows; i++)
			{
				intPtr = WindowsApi.GetWindow(intPtr, GetWindowCmd.GwHwndnext);
				if (intPtr == PptOperator.Instance.SlideShowWindowIntPtr)
				{
					return;
				}
			}
			WindowsApi.SetWindowPos(new WindowInteropHelper(window).Handle, -1, 0, 0, 0, 0, 3);
		}
	}
}
