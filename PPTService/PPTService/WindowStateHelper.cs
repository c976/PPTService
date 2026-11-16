using System;
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
			PptOperator op = PptOperator.Instance;
			if (op == null || !op.IsShowing)
			{
				return false;
			}
			IntPtr hwnd = op.SlideShowWindowIntPtr;
			if (hwnd == IntPtr.Zero || !WindowsApi.IsWindow(hwnd) || !WindowsApi.IsWindowVisible(hwnd))
			{
				return false;
			}

			// 旧实现用 WindowFromPoint 在窗口四角/中心做命中测试判断「放映是否仍在前台」。
			// 覆盖层改成附着放映窗口后不再全局置顶，PPT 一失去焦点（用户打开别的文件、
			// 点到任务栏等）四个采样点就命中别的窗口，工具栏被置成 Opacity=0 看不见，
			// 要等焦点回来才恢复——正是「启动时偶尔看不到」的根因。
			// 现在 ShowBegin/ShowEnd + 放映 HWND 已是可靠真值源，不再按焦点隐藏工具栏。
			// 仅保留原版对 remoteshow 特殊窗口的处理。
			try
			{
				IntPtr intPtr = WindowsApi.WindowFromPoint(new MousePoint(window.Left, window.Top));
				IntPtr intPtr2 = WindowsApi.WindowFromPoint(new MousePoint(window.Left + window.Width - (double)Offset, window.Top + (double)Offset));
				if (intPtr == intPtr2)
				{
					Sb.Clear();
					WindowsApi.GetWindowText(intPtr, Sb, Sb.Capacity);
					if (Sb.ToString().ToLower() == RemoteShow)
					{
						WindowsApi.SetWindowPos(intPtr, 0, 0, 1, 0, 0, 69);
					}
				}
			}
			catch
			{
				// 命中测试失败不影响放映状态判断。
			}
			return true;
		}

		private static IntPtr _attachedWindow;
		private static IntPtr _attachedOwner;

		/// <summary>
		/// 让覆盖层「只附着在 PPT 放映窗口上」，而不是全局置顶。
		///
		/// 用 Win32 Owner 关系（WindowInteropHelper.Owner）：owned window 始终位于 owner 之上，
		/// 但不属于 HWND_TOPMOST 组，因此：
		///   - 用户在 PPT 上再开一个窗口（含第三方置顶窗口）时能正常盖住批注层、也能拿到焦点；
		///   - Win+Tab / 虚拟桌面会把批注层与 PPT 放映窗口归为同一组，一起移动、一起预览；
		///   - 不再反复 SetWindowPos(HWND_TOPMOST) 抢 z 序与前台焦点。
		///
		/// 由计时器每次 tick 调用，幂等；放映窗口句柄一旦失效立即解除 Owner，
		/// 避免 owner 被系统销毁时把本窗口一起销毁。
		/// </summary>
		public static void MakeAppAbovePpt(Window window)
		{
			if (window == null)
			{
				return;
			}
			IntPtr hwnd = new WindowInteropHelper(window).Handle;
			if (hwnd == IntPtr.Zero)
			{
				return;
			}
			IntPtr owner = PptOperator.Instance == null
				? IntPtr.Zero
				: PptOperator.Instance.SlideShowWindowIntPtr;
			if (owner == IntPtr.Zero || owner == hwnd || !WindowsApi.IsWindow(owner))
			{
				DetachFromPpt(window);
				return;
			}
			if (_attachedWindow == hwnd && _attachedOwner == owner)
			{
				return;
			}
			try
			{
				new WindowInteropHelper(window).Owner = owner;
				_attachedWindow = hwnd;
				_attachedOwner = owner;
				// 双保险：即使 XAML/样式仍带置顶位，也强制清掉，确保不在全局置顶组。
				uint exStyle = WindowsApi.GetWindowLong(hwnd, WindowsApi.WindowFlags.GwlExstyle);
				if ((exStyle & WindowsApi.WindowFlags.WsExTopmost) != 0u)
				{
					WindowsApi.SetWindowLong(hwnd, WindowsApi.WindowFlags.GwlExstyle,
						exStyle & ~WindowsApi.WindowFlags.WsExTopmost);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Attach to slide show window failed: " + ex);
			}
		}

		/// <summary>放映结束 / 放映窗口销毁前解除 Owner，避免本窗口被连带销毁。</summary>
		public static void DetachFromPpt(Window window)
		{
			if (window == null)
			{
				return;
			}
			try
			{
				WindowInteropHelper helper = new WindowInteropHelper(window);
				if (helper.Handle != IntPtr.Zero)
				{
					helper.Owner = IntPtr.Zero;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Detach from slide show window failed: " + ex);
			}
			_attachedWindow = IntPtr.Zero;
			_attachedOwner = IntPtr.Zero;
		}
	}
}
