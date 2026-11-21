using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace PPTService
{
	/// <summary>
	/// 批注层控制器：把 PPT 小工具的工具栏（鼠标 / 笔 / 橡皮 / 颜色）与 PPT 翻页事件
	/// 接到 PPTService.Ink 里的原版桌面批注白板 Board 控件上。
	///
	/// 与原版桌面批注一致的能力：
	///  - 压感软笔：DefaultDesktopBoardModesProvider 的 SoftPenInputFromBoardWritingMode；
	///  - 手势橡皮：EraserMode + EraserGestureFilter（Initializer 里注册）；
	///  - 鼠标模式：Board.IsHitTestVisible=false，点击穿透到 PPT；
	///  - 逐页笔迹：GoToSlide，PPT 翻页（含不通过小工具的翻页）时同步切换。
	///
	/// 批注层（Board 控件）直接挂在小工具主窗口 MainWindow 的视觉树里、处于最底层，
	/// 工具栏按钮在其之上——WPF 单窗口命中测试让按钮永远可点、空白处才落到 Board，
	/// 不再需要独立的 Topmost 窗口与 SetWindowPos 跨窗口抢 z 序（此前正是这里引发了
	/// 触控画第一笔卡死与「批注层盖住按钮」）。
	/// </summary>
	public static class AnnotationController
	{
		public enum Mode
		{
			Mouse,
			Pen,
			Eraser
		}

		private static AnnotationBoard _board;
		private static Window _toolbarWindow;
		private static int _currentPageIndex = 1;
		private static Color _color = PenSettings.DefaultColor;
		private static double _thickness = PenSettings.PenWeight;
		private static Mode _currentMode = Mode.Mouse;

		public static bool IsEnabled { get; private set; }

		/// <summary>当前笔迹颜色（自实现批注的真值来源；Office 指针类型为 None 时 PptOperator.PointerColor 不可信）。</summary>
		public static Color CurrentColor
		{
			get { return _color; }
		}

		/// <summary>当前笔迹粗细（移植自桌面批注的三档粗细，两个白板共用）。</summary>
		public static double CurrentThickness
		{
			get { return _thickness; }
		}

		public static Mode CurrentMode
		{
			get { return _currentMode; }
			private set { _currentMode = value; }
		}

		/// <summary>
		/// 传入小工具主窗口：批注层 Board 作为它视觉树最底层的子元素挂入。
		/// </summary>
		public static void Attach(Window toolbarWindow)
		{
			_toolbarWindow = toolbarWindow;
			if (_toolbarWindow == null)
			{
				return;
			}
			_toolbarWindow.Closed += delegate { Close(); };
			try
			{
				_currentPageIndex = Math.Max(1, PptOperator.Instance.CurrentShowPosition);
			}
			catch
			{
				_currentPageIndex = 1;
			}
		}

		private static MainWindow Toolbar
		{
			get { return _toolbarWindow as MainWindow; }
		}

		private static void EnsureBoard()
		{
			if (_board != null)
			{
				return;
			}
			_board = new AnnotationBoard();
			// 这里不再负责挂层：挂层统一放到 SetEnabled()，因为它依赖 Toolbar，
			// 而 Toolbar 可能晚于 Board 出现（见下方 SetEnabled 的说明）。
			// 把用户当前选择的颜色 / 粗细应用到新建的 Board
			// （Board 构造里用的是默认红色与默认粗细）。
			_board.SetInkColor(_color);
			_board.SetInkThickness(_thickness);
		}

		public static void SetEnabled(bool enable)
		{
			IsEnabled = enable;
			if (!enable)
			{
				Close();
				return;
			}
			// 自实现批注启用时，Office/WPS 自身的指针必须回到「无」，
			// 否则鼠标模式下点击穿透到 PPT 会同时触发 Office 原生画笔。
			try
			{
				PptOperator.Instance.PointerType =
					Microsoft.Office.Interop.PowerPoint.PpSlideShowPointerType.ppSlideShowPointerNone;
			}
			catch
			{
				// 放映未开始，忽略。
			}
			PptOperator.Instance.PointerColor = _color;
			EnsureBoard();
			if (_board != null)
			{
				// 每次 SetEnabled 都确保 Board 挂到当前工具栏窗口（幂等）。
				// 关键场景：/settingmode 冷启动时没有 PPT、也就没有工具栏，
				// 但打开设置仍会走 ApplyCurrent → SetEnabled(true) 创建 Board，
				// 那时 Toolbar=null 无法挂层，Board 悬空。之后放映创建工具栏再
				// SetEnabled(true) 时 EnsureBoard 因 _board 已存在直接返回，若挂层
				// 还留在 EnsureBoard 里就永远不会补挂，表现为笔和橡皮完全写不出。
				// 现在把挂层移到这里，放映时必定重新挂到新的 MainWindow。
				MainWindow main = Toolbar;
				if (main != null)
				{
					main.SetAnnotationLayer(_board.InkBoard);
				}
				_board.InkBoard.Visibility = Visibility.Visible;
				_board.GoToSlide(_currentPageIndex - 1);
				_lastFocusMode = null;
				SetMode(_currentMode);
			}
		}

		/// <summary>放映结束只隐藏批注层，不改变用户的批注方式配置。</summary>
		public static void Hide()
		{
			if (_board != null && _board.InkBoard != null)
			{
				_board.InkBoard.Visibility = Visibility.Collapsed;
			}
		}

		/// <summary>
		/// 放映结束 / 放映中断：把批注模式复位成「鼠标」。
		/// 工具栏的 UpdateButtonState(None) 只改了 UI，不改这里的话下次放映 SetEnabled()
		/// 会用旧的 _currentMode(Pen/Eraser) 把 Board 切回笔，出现「界面显示鼠标、实际是笔」。
		/// </summary>
		public static void ResetModeToMouse()
		{
			// 只复位状态与已有 Board，不走 SetMode → EnsureBoard，
			// 避免在放映结束时反而重新创建/挂上批注层。
			_currentMode = Mode.Mouse;
			_lastFocusMode = null;
			if (_board != null)
			{
				_board.GotoSelect();
			}
		}

		public static void Close()
		{
			if (_board != null)
			{
				MainWindow main = Toolbar;
				if (main != null)
				{
					main.RemoveAnnotationLayer();
				}
				_board = null;
			}
		}

		private static Mode? _lastFocusMode;

		public static void SetMode(Mode mode)
		{
			_currentMode = mode;
			if (!IsEnabled)
			{
				return;
			}
			EnsureBoard();
			if (_board == null)
			{
				return;
			}
			if (mode == Mode.Mouse)
			{
				_board.GotoSelect();
			}
			else if (mode == Mode.Pen)
			{
				_board.GotoInk();
			}
			else
			{
				_board.GotoEraser();
			}
			ApplyFocusForMode(mode);
		}

		/// <summary>
		/// 让前台焦点跟随批注模式，解决「附着模式下手写横滑误触发 PowerPoint 翻页」：
		/// PowerPoint 只在自己是前台活动窗口时才响应左右滑动翻页手势。
		/// 笔 / 橡皮模式必须让批注窗口持有焦点，PPT 失焦后手写横滑就不会被它当成翻页；
		/// 鼠标模式把焦点还给 PPT，保留 PowerPoint 原生翻页（方向键 / 翻页笔 / 触屏滑动），
		/// 与全局置顶模式的表现一致。仅在模式真正变化时设一次，不每 tick 抢焦点。
		/// debug 模拟放映没有真实 PPT 窗口，跳过。
		/// </summary>
		private static void ApplyFocusForMode(Mode mode)
		{
			if (DebugMode.IsEnabled || _toolbarWindow == null)
			{
				return;
			}
			if (_lastFocusMode == mode)
			{
				return;
			}
			_lastFocusMode = mode;
			try
			{
				IntPtr hwnd = new WindowInteropHelper(_toolbarWindow).Handle;
				if (mode == Mode.Mouse)
				{
					// 鼠标模式：焦点还给 PPT，PowerPoint 原生翻页照常。
					IntPtr ppt = PptOperator.Instance.SlideShowWindowIntPtr;
					if (ppt != IntPtr.Zero)
					{
						WindowsApi.SetForegroundWindow(ppt);
					}
				}
				else if (hwnd != IntPtr.Zero)
				{
					// 笔 / 橡皮：批注窗口持有焦点，PPT 失焦 → 手写横滑不被识别成翻页手势。
					WindowsApi.SetForegroundWindow(hwnd);
					// SetForegroundWindow 只设「前台窗口」，不保证键盘焦点落在 WPF 窗口上。
					// 若键盘焦点不在 MainWindow，MainWindow_KeyDown 收不到翻页键，
					// 自实现批注下翻页笔/键盘翻页就失效（鼠标模式焦点在 PPT、走原生翻页故正常）。
					// 显式在 UI 线程把键盘焦点给工具栏，让翻页键能被捕获并转发到 PPT COM。
					System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
					{
						try
						{
							_toolbarWindow.Focus();
							// MainWindow 样式原本 Focusable=False，Focus() 会静默失败；
							// XAML 已覆盖为 True，这里再用 Keyboard.Focus 确保键盘焦点落到工具栏，
							// 否则 PreviewKeyDown 收不到翻页键（自实现批注下翻页笔失效的根因）。
							System.Windows.Input.Keyboard.Focus(_toolbarWindow);
						}
						catch
						{
							// 焦点设置失败不影响批注本身。
						}
					}));
				}
			}
			catch (Exception)
			{
				// 焦点设置失败不影响批注本身。
			}
		}

		public static void SetColor(Color color)
		{
			_color = color;
			// 定时器在自实现批注下不再调 PptOperator.UpdatePointerColor()（COM），
			// 这里手动同步普通属性，保证颜色盘与板中板画笔颜色正确。
			// PptOperator.Instance 由 App.OnStartup 赋值，启动早期可能为 null。
			if (PptOperator.Instance != null)
			{
				PptOperator.Instance.PointerColor = color;
			}
			if (_board != null)
			{
				_board.SetInkColor(color);
			}
		}

		/// <summary>设置笔迹粗细，同步主批注层与板中板（两者共用同一真值源）。</summary>
		public static void SetThickness(double thickness)
		{
			_thickness = thickness;
			if (_board != null)
			{
				_board.SetInkThickness(thickness);
			}
		}

		/// <summary>清除当前页全部笔迹（对应桌面批注橡皮弹窗的滑杆清屏）。</summary>
		public static void ClearCurrentPage()
		{
			if (_board != null)
			{
				_board.CleanInk();
			}
		}

		/// <summary>
		/// 重新放映 / 切换到新 PPT 时清掉上一场的全部笔迹。
		/// Board 按页累计 Slide，若不清除，结束放映再放映、或换一份 PPT 时旧笔迹仍会残留。
		/// 一场放映内的翻页只走 ShowPage（保留逐页笔迹），不调用本方法。
		/// </summary>
		public static void Reset()
		{
			if (_board != null)
			{
				_board.ClearSlides();
			}
		}

		/// <summary>
		/// 放映结束：把笔色与粗细复位为默认值，并同步到主批注层 Board 与 Office 指针色缓存。
		/// 只改纯状态与已有 Board，不走 SetColor → 避免在放映结束时反而重新挂上批注层。
		/// 板中板有自己独立的 Board，由 InnerBoardWindow.ResetPenSettings() 单独复位。
		/// </summary>
		public static void ResetPenSettings()
		{
			_color = PenSettings.DefaultColor;
			_thickness = PenSettings.PenWeight;
			if (PptOperator.Instance != null)
			{
				// 同时复位 Office 原生画笔颜色缓存与 COM 指针色（不改指针类型）。
				PptOperator.Instance.ResetPointerColor();
			}
			if (_board != null)
			{
				_board.SetInkColor(_color);
				_board.SetInkThickness(_thickness);
			}
		}

		/// <summary>
		/// 判断一次鼠标按下的来源是否落在自实现批注的绘制层 Board 上。
		/// 用来区分「用户确实要在幻灯片上落第一笔」与「误点弹窗 / 工具栏 / 窗口空白」：
		/// 只有落在 Board 的按下才收起颜色盘 / 清屏滑杆，其余位置一律不关，避免误触。
		/// </summary>
		public static bool IsPointerOnAnnotationBoard(DependencyObject source)
		{
			var board = _board == null ? null : _board.InkBoard;
			DependencyObject dep = source;
			if (board != null)
			{
				while (dep != null)
				{
					if (ReferenceEquals(dep, board))
					{
						return true;
					}
					dep = VisualTreeHelper.GetParent(dep);
				}
				return false;
			}
			// Office 原生批注下没有批注层：退回「点击落在主窗口可视树」的判定，
			// 保证点窗口外仍能收起弹窗。Popup 内容在独立 HWND 的可视树里（根不是
			// Window），因此弹窗内空白不会命中；工具栏按钮要排除，否则会破坏开合 toggle。
			while (dep != null)
			{
				if (dep is ButtonBase || dep is Slider || dep is Thumb)
				{
					return false;
				}
				if (dep is Window)
				{
					return true;
				}
				dep = VisualTreeHelper.GetParent(dep);
			}
			return false;
		}

		/// <summary>PPT 翻页（含不通过小工具的翻页）时切换到对应页的笔迹。</summary>
		public static void ShowPage(int pageIndex)
		{
			if (pageIndex < 1)
			{
				pageIndex = 1;
			}
			_currentPageIndex = pageIndex;
			if (_board != null)
			{
				_board.GoToSlide(pageIndex - 1);
			}
		}
	}
}
