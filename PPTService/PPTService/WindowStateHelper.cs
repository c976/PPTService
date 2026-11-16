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
			// 工具栏显隐只认 ShowBegin/ShowEnd 维护的放映状态这一权威真值源。
			//
			// 曾经额外用 IsWindow/IsWindowVisible 检查放映窗口 HWND：放映刚启动、
			// 切显示器或 COM 忙时该 HWND 会短暂失效/不可见，这里就返回 false，
			// 把工具栏置成 Opacity=0 看不见；用户在 PPT 页面右键一下会激活放映
			// 窗口、泵通 COM 消息，HWND 恢复可见，下一 tick 工具栏又冒出来——
			// 这正是「放 PPT 后批注栏不显示、右键一下才出现」，且全程无异常、
			// log 干净。放映窗口 HWND 的生命周期已由 MakeAppAbovePpt/DetachFromPpt
			// 负责，这里不再重复按瞬时窗口状态隐藏工具栏。
			if (op == null || !op.IsShowing)
			{
				return false;
			}
			IntPtr hwnd = op.SlideShowWindowIntPtr;

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
		/// 应用「批注窗口层级」用户开关，即时生效且可反复调用。
		/// false（默认）= 解除全局置顶、附着到 PPT 放映窗口（Owner）；
		/// true = 回退旧版全局置顶（Topmost），不使用 Owner。
		/// </summary>
		public static void ApplyWindowLayerMode(Window window)
		{
			if (window == null)
			{
				return;
			}
			bool legacy = UserConfigManager.Load().LegacyTopmostWindow;
			IntPtr hwnd = new WindowInteropHelper(window).Handle;
			if (legacy)
			{
				// 从「附属 PPT」切到旧版：先卸掉 Owner，否则 owner 仍会约束 z 序。
				DetachFromPpt(window);
				if (hwnd == IntPtr.Zero)
				{
					return;
				}
				try
				{
					// WPF 的 Topmost 属性会同步设置 WS_EX_TOPMOST；
					// 再显式推一次 HWND_TOPMOST，保证立即压到最上层。
					if (!window.Topmost)
					{
						window.Topmost = true;
					}
					WindowsApi.SetWindowPos(hwnd, -1, 0, 0, 0, 0, 0x0003);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("Enable legacy topmost failed: " + ex);
				}
				return;
			}

			// 从旧版切回默认：先清全局置顶位，再（若正在放映）重新挂 Owner。
			try
			{
				if (window.Topmost)
				{
					window.Topmost = false;
				}
				if (hwnd != IntPtr.Zero)
				{
					uint exStyle = WindowsApi.GetWindowLong(hwnd, WindowsApi.WindowFlags.GwlExstyle);
					if ((exStyle & WindowsApi.WindowFlags.WsExTopmost) != 0u)
					{
						WindowsApi.SetWindowLong(hwnd, WindowsApi.WindowFlags.GwlExstyle,
							exStyle & ~WindowsApi.WindowFlags.WsExTopmost);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Disable legacy topmost failed: " + ex);
			}
			MakeAppAbovePpt(window);
		}

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
			// 旧版全局置顶模式：不挂 Owner；若之前处于附属模式则解除，并保持置顶位。
			if (UserConfigManager.Load().LegacyTopmostWindow)
			{
				if (_attachedWindow == hwnd || _attachedOwner != IntPtr.Zero)
				{
					DetachFromPpt(window);
				}
				// 旧版行为：每次 tick 都把窗口推回 HWND_TOPMOST（不移动/不改尺寸/不抢焦点），
				// 这样即使第三方置顶窗口抢占了层级，下一 tick 也会恢复成批注窗口在上。
				WindowsApi.SetWindowPos(hwnd, -1, 0, 0, 0, 0, 0x0003);
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
				// Owner 关系已建立，但 PowerPoint 放映窗口是 topmost，放映过程中会
				// 反复把自己提到最前，连带把作为 owned window 的覆盖层压到它下面。
				// 用户右键放映窗口 = 激活 owner，Windows 此时会把 owned window 带回
				// owner 之上，于是工具栏「冒出来」——正是「放 PPT 后看不到批注栏，
				// 右键一下才显示」。所以每次 tick 都要把本窗口重新插到 owner 之上
				// （HWND_TOPMOST=-1 会全局置顶，这里用 owner 作为插入点，只保证
				// 在放映窗口之上、不抢其它程序焦点）。幂等分支不可直接 return。
				KeepAboveOwner(hwnd, owner);
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
				KeepAboveOwner(hwnd, owner);
				Log.WriteDebug($"[ZORDER] attach owner={owner} overlay={hwnd} topmost={(WindowsApi.GetWindowLong(hwnd, WindowsApi.WindowFlags.GwlExstyle) & WindowsApi.WindowFlags.WsExTopmost) != 0u}");
			}
			catch (Exception ex)
			{
				Log.WriteDebug("Attach to slide show window failed: " + ex);
			}
		}

		// owner topmost 状态诊断：只在变化时写，避免每 tick 刷屏。
		private static bool? _lastOwnerTopmost;

		/// <summary>
		/// 把覆盖层维持在放映窗口之上。
		/// 关键：PowerPoint 放映窗口自身是 HWND_TOPMOST。Windows 的 z 序规则是
		/// 「所有 topmost 窗口恒定压在所有非 topmost 窗口之上」，所以用 owner 作为
		/// SetWindowPos 的插入点是没用的——覆盖层仍属于非 topmost 组，会被放映窗口
		/// 压在下面；只有右键激活 owner 时 Windows 才临时把 owned window 带上来，
		/// 随即又被盖回去（右键后显示、一闪而过）。
		/// 因此当 owner 带 WS_EX_TOPMOST 时，覆盖层必须也用 HWND_TOPMOST 才能真正
		/// 待在它上面；owner 非 topmost 时才退回「插到 owner 之上」的普通层级。
		/// 这不会抢焦点：始终带 SWP_NOACTIVATE，只调 z 序，不改尺寸/位置。
		/// </summary>
		private static void KeepAboveOwner(IntPtr hwnd, IntPtr owner)
		{
			if (hwnd == IntPtr.Zero || owner == IntPtr.Zero)
			{
				return;
			}
			uint ownerExStyle = WindowsApi.GetWindowLong(owner, WindowsApi.WindowFlags.GwlExstyle);
			bool ownerTopmost = (ownerExStyle & WindowsApi.WindowFlags.WsExTopmost) != 0u;
			if (_lastOwnerTopmost != ownerTopmost)
			{
				_lastOwnerTopmost = ownerTopmost;
				Log.WriteDebug($"[ZORDER] ownerTopmost={ownerTopmost} ownerEx={ownerExStyle:X} hwnd={hwnd}");
			}
			// 0x0003 = SWP_NOSIZE|SWP_NOMOVE，0x0010 = SWP_NOACTIVATE。
			int flags = 0x0001 | 0x0002 | 0x0010;
			if (ownerTopmost)
			{
				WindowsApi.SetWindowPos(hwnd, -1, 0, 0, 0, 0, flags);
			}
			else
			{
				WindowsApi.SetWindowPos(hwnd, owner, 0, 0, 0, 0, flags);
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
