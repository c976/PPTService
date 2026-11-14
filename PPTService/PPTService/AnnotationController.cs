using System;
using System.Windows;
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
			MainWindow main = Toolbar;
			if (main != null)
			{
				main.SetAnnotationLayer(_board.InkBoard);
			}
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
				_board.InkBoard.Visibility = Visibility.Visible;
				_board.GoToSlide(_currentPageIndex - 1);
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
