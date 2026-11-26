using System;
using System.Windows;
using System.Windows.Interop;
using PPTServiceUltra.Core.Windows;

namespace PPTServiceUltra
{
	public class ViewHelper
	{
		private static InnerBoardWindow _innerBoardWindow;

		public static Magnifier Magnifier { get; set; }

		public static LeftArea LeftArea { get; private set; }

		public static RightArea RightArea { get; private set; }

		public static BottomArea BottomArea { get; private set; }

		public static LeftBottomArea LeftBottomArea { get; private set; }

		public static RightBottomArea RightBottomArea { get; private set; }

		public static MainWindow MainWindow { get; private set; }

		/// <summary>
		/// 设置窗口是否正在显示。设置窗口是全屏半透明遮罩，打开期间必须彻底停掉
		/// 横滑翻页手势：单纯 Stop() 一次会被 MainWindow 的 300ms 跟随定时器重新 Start()，
		/// 而且 PowerPoint 原生手势按触点命中窗口投递、抢焦点并不可靠。这里用一个
		/// UI 线程读写的标志，让定时器和 Gesture 翻页判断都据此持续抑制。
		/// </summary>
		public static bool SettingsWindowOpen { get; private set; }

		public static InnerBoardWindow InnerBoardWindow
		{
			get
			{
				if (_innerBoardWindow != null)
				{
					return _innerBoardWindow;
				}
				_innerBoardWindow = new InnerBoardWindow
				{
					Owner = MainWindow
				};
				ControlHelper.SetAltF4Enabled(InnerBoardWindow, false);
				return _innerBoardWindow;
			}
		}

		/// <summary>放映结束时复位板中板笔色 / 粗细；窗口尚未创建则不主动创建（避免建一整套白板）。</summary>
		public static void ResetInnerBoardPenSettingsIfCreated()
		{
			if (_innerBoardWindow != null)
			{
				_innerBoardWindow.ResetPenSettings();
			}
		}

		public void Initialize()
		{
			PptOperator.Instance.ShowBegin += Instance_ShowBegin;
		}

		private void Instance_ShowBegin(object sender, EventArgs e)
		{
			// 不再退订事件：旧逻辑在第一次 ShowBegin 就退订，若这次创建窗口的过程中
			// 抛异常（LoadingScreen / LeftArea 构造失败等），MainWindow 保持 null 但 handler
			// 已消失，此后每场放映都没有人能创建工具栏，必须重启进程才能恢复。
			// 改为常驻订阅 + 下面的「窗口已存在只刷新」判重，既能防重复创建，也能自愈重试。
			SyntacticSugar.CurrentDispatcherInvoke(() =>
			{
				// 防御性兜底：窗口已存在时只刷新它，绝不创建第二份工具栏实例。
				if (MainWindow != null)
				{
					MainWindow._pptOperatorShowBegin(null, null);
					return;
				}
				LoadingScreen<LoadingView>.Current = new LoadingScreen<LoadingView>();
				LeftArea = new LeftArea();
				RightArea = new RightArea();
				LeftBottomArea = new LeftBottomArea();
				RightBottomArea = new RightBottomArea();
				BottomArea = new BottomArea();
				MainWindow = new MainWindow();
				ControlHelper.SetAltF4Enabled(MainWindow, false);
				MainWindow._pptOperatorShowBegin(null, null);
			});
		}

		public static void StopMainWindowUpdateTimerAndMouseGesture()
		{
			MainWindow.IsNeedUpdateTimer = false;
			MainWindow.UpdateTimer.Stop();
			MouseHelper.Instance.Stop();
		}

		public static void BeginMainWindowUpdateTimerAndMouseGesture()
		{
			MainWindow.IsNeedUpdateTimer = true;
			MainWindow.UpdateTimer.Start();
			MouseHelper.Instance.Start();
		}

		/// <summary>
		/// 统一打开设置窗口（工具栏按钮与 /settingmode 调用共用）。
		/// 设置窗口是全屏半透明遮罩，打开期间用户要能看到主界面的调节效果，
		/// 但不能再拖动 PPT：这里停掉鼠标手势（含横滑翻页/画线跟踪）并让主界面
		/// 不再接收输入；跟随 PPT 的 300ms 定时器保持运行，不影响位置同步。
		/// 设置窗口关闭后在 finally 中无条件恢复，避免异常导致工具栏永久失效。
		/// </summary>
		public static void OpenSettingsWindow()
		{
			MainWindow mainWindow = MainWindow;
			bool wasHitTestVisible = mainWindow != null && mainWindow.IsHitTestVisible;
			// 置位标志：即使 300ms 定时器重新走到 UpdateGestureAndMouseLineTrackState，
			// 也会因该标志保持 MouseHelper 停止；Gesture 翻页判断同样据此跳过。
			SettingsWindowOpen = true;
			// 仅在放映中（工具栏已创建）才干预鼠标手势与主界面输入；
			// settingmode 冷启动时 MainWindow 为 null，不做任何句柄级操作。
			if (mainWindow != null)
			{
				MouseHelper.Instance.Stop();
				mainWindow.IsHitTestVisible = false;
			}
			try
			{
				SettingsWindow settingsWindow = new SettingsWindow
				{
					Owner = mainWindow
				};
				// 放映中打开设置：设置窗口是全屏半透明遮罩，若 PPT 放映窗口仍是前台，
				// 用户在遮罩上的左右滑动会被 PowerPoint 当成原生翻页手势而误翻页。
				// 这里显式把前台焦点夺到设置窗口，让 PPT 失焦；关闭后再按批注模式恢复。
				if (mainWindow != null)
				{
					settingsWindow.Loaded += delegate
					{
						SyntacticSugar.TryCatchAction(() =>
						{
								IntPtr handle = new WindowInteropHelper(settingsWindow).Handle;
							if (handle != IntPtr.Zero)
							{
								WindowsApi.SetForegroundWindow(handle);
							}
							// 设置窗口是全屏遮罩：PPT 可能刚把指针藏起来，这里立刻补一次，
							// 避免用户打开设置后看不到指针位置；放映定时器后续也会持续兜底。
							CursorVisibilityHelper.EnsureVisible();
						});
					};
				}
				settingsWindow.ShowDialog();
			}
			finally
			{
				// 无论是否放映中都要复位，避免设置窗口关闭后手势永久停摆。
				SettingsWindowOpen = false;
				if (mainWindow != null)
				{
					mainWindow.IsHitTestVisible = wasHitTestVisible;
					// 关闭设置后恢复焦点：鼠标模式下交还 PPT 走原生翻页；
					// 自实现批注下交还工具栏，由 MainWindow_KeyDown 转发翻页键。
					RestoreFocusAfterSettings();
					// 只有当前没有其它弹窗需要停手势时才恢复；跟随定时器未停，
					// 下次 tick 的 UpdateGestureAndMouseLineTrackState 也会按状态重新校准。
					if (BottomArea == null || !BottomArea.IsNeedStopMouseLineTrack)
					{
						MouseHelper.Instance.Start();
					}
				}
			}
		}

		/// <summary>
		/// 设置窗口关闭后恢复前台焦点：
		///  - 自实现批注开启：交还工具栏（BAML 已设 Focusable=True），翻页键由 MainWindow_KeyDown 转发 COM；
		///  - 关闭自实现批注 / 未放映：交还 PPT 放映窗口，保留 PowerPoint 原生翻页。
		/// 设置窗口刚关闭时焦点可能仍在已关闭窗口上，必须显式重设，否则两边都收不到翻页键。
		/// </summary>
		private static void RestoreFocusAfterSettings()
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				bool selfAnnotation = UserConfigManager.Load().UseSelfAnnotation;
				IntPtr target = IntPtr.Zero;
				if (selfAnnotation && MainWindow != null)
				{
					target = new WindowInteropHelper(MainWindow).Handle;
				}
				if (target == IntPtr.Zero && PptOperator.Instance != null)
				{
					target = PptOperator.Instance.SlideShowWindowIntPtr;
				}
				if (target != IntPtr.Zero)
				{
					WindowsApi.SetForegroundWindow(target);
				}
			});
		}
	}
}
