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
		private static Color _color = Colors.Red;
		private static Mode _currentMode = Mode.Mouse;

		public static bool IsEnabled { get; private set; }

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
			PptOperator.Instance.PointerColor = color;
			if (_board != null)
			{
				_board.SetInkColor(color);
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
