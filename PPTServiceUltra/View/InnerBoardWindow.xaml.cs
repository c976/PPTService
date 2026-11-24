using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Office.Interop.PowerPoint;
using PPTServiceUltra.Collection;

namespace PPTServiceUltra
{
	/// <summary>
	/// 板中板：与主批注层共用 PPTServiceUltra.Ink 的新白板引擎（AnnotationBoard），
	/// 不再使用旧的 System.Windows.Controls.InkCanvas + StrokeCollection[] 手写分页。
	/// 好处：板中板同样获得压感软笔、手势橡皮、触控 / 鼠标输入与逐页笔迹（Board.Slides），
	/// 且与主批注层共用同一套颜色 / 模式语义。
	///
	/// 本窗口有白色不透明底板（InkCanvasBorder），分层窗口在该区域的像素 alpha=255，
	/// 输入天然会被捕获，因此 AnnotationBoard 传 captureInputWithAlpha=false：
	/// Board 铺白底即可命中，不需要 MainWindow 那种 alpha=1 捕获技巧，也不会给 PPT 带色偏。
	/// </summary>
	public partial class InnerBoardWindow : Window, IComponentConnector
	{
		private int _totalIndex = 1;

		private int _currentIndex = 1;

		private static readonly int MaxSlides = 100;

		/// <summary>板中板自己的白板实例（Board 是独立 UIElement，不能与 MainWindow 共用）。</summary>
		private readonly AnnotationBoard _board;

		public bool IsShowing { get; set; }

		/// <summary>当前是否处于橡皮模式（供工具栏判断「再点一次橡皮」）。</summary>
		public bool IsEraserActive { get; private set; }

		public InnerBoardWindow()
		{
			InitializeComponent();
			_board = new AnnotationBoard(false);
			// 放在页码按钮之前插入：Board 在 Grid 里声明顺序早于 SlideIndexButtonBorder，
			// 因此按钮永远在笔迹层之上，不会出现「画到按钮上就点不了」。
			BoardHost.Children.Add(_board.InkBoard);
			Loaded += InnerBoardWindow_Loaded;
			CloseBorder.MouseLeftButtonDown += (object s, MouseButtonEventArgs e) =>
			{
				HiddenWindow();
				FridayReporter.ReportEventImmediately(EventID.InnerBoardExit);
			};
			// 与 PPT 主批注层一致：颜色盘/清屏滑杆打开时，点击板中板绘图区的第一击
			// 要关掉弹窗并继续落到本窗口 Board 上落笔/擦除。MainWindow 的
			// PreviewMouseDown 只认主批注 Board，板中板是独立窗口收不到，故在此补一份。
			// 只关弹窗、不设 e.Handled，让点击继续路由到 Board。
			PreviewMouseDown += InnerBoardWindow_PreviewMouseDown;
		}

		private void InnerBoardWindow_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			BottomArea area = ViewHelper.BottomArea;
			if (area == null || !area.HasOpenToolPopup)
			{
				return;
			}
			// 只有落在板中板自己的批注 Board 上才关弹窗；页码按钮、关闭按钮、
			// 板中板空白区都不关，避免误触打断弹窗操作。
			System.Windows.Media.Visual board = _board == null ? null : _board.InkBoard;
			DependencyObject dep = e.OriginalSource as DependencyObject;
			while (dep != null)
			{
				if (ReferenceEquals(dep, board))
				{
					area.CloseToolPopups();
					return;
				}
				dep = System.Windows.Media.VisualTreeHelper.GetParent(dep);
			}
		}

		private void InnerBoardWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Top = 0.0;
			// 与主批注层共用颜色 / 粗细真值源，不再使用旧 InkCanvas 时代的固定笔宽。
			_board.SetInkThickness(AnnotationController.CurrentThickness);
			_board.SetInkColor(AnnotationController.CurrentColor);
		}

		/// <summary>清空所有页笔迹并回到第 1 页（关闭 / 退出放映时调用）。</summary>
		public void ClearInkCanvas()
		{
			_board.ClearSlides();
			// Clear 会插入新页并删除旧页，显式回到第 1 页保证与 _currentIndex 同步。
			_board.GoToSlide(0);
			_totalIndex = 1;
			_currentIndex = 1;
			IsEraserActive = false;
			UpdateIndex();
		}

		/// <summary>
		/// 放映结束：把板中板自己的 Board 笔色 / 粗细复位为全局默认值。
		/// 板中板与主批注层是两个独立 Board，仅复位 AnnotationController 不够。
		/// </summary>
		public void ResetPenSettings()
		{
			IsEraserActive = false;
			_board.SetInkColor(AnnotationController.CurrentColor);
			_board.SetInkThickness(AnnotationController.CurrentThickness);
			_board.GotoSelect();
		}

		/// <summary>「滑动清屏」：只清当前页笔迹，保留其它页（对应桌面批注的 CleanInk）。</summary>
		public void ClearCurrentPage()
		{
			_board.CleanInk();
		}

		/// <summary>设置板中板笔迹粗细。</summary>
		public void SetPenThickness(double thickness)
		{
			_board.SetInkThickness(thickness);
		}

		/// <summary>切到画笔模式（沿用当前笔色）。</summary>
		public void SetPenToInkMode()
		{
			IsEraserActive = false;
			_board.GotoInk();
		}

		/// <summary>设置笔色并切到画笔模式。</summary>
		public void SetPen(Color color)
		{
			IsEraserActive = false;
			_board.SetInkColor(color);
			_board.GotoInk();
		}

		/// <summary>手势橡皮（拖动擦除）。</summary>
		public void SetRubber()
		{
			IsEraserActive = true;
			_board.GotoEraser();
		}

		/// <summary>鼠标 / 选择模式：白板不再接收笔迹输入。</summary>
		public void SetNone()
		{
			IsEraserActive = false;
			_board.GotoSelect();
		}

		public void ShowWindow()
		{
			if (!IsShowing)
			{
				IsShowing = true;
				Show();
				ChangeWindowRect();
				// 每一次进入板中板都重置为「画笔」：同步当前颜色 / 粗细、清除橡皮态、切到 GotoInk。
				// 不依赖 ShowStoryBoard 的 Completed 回调，避免动画被打断时停留在上次的橡皮 / 选择模式。
				_board.SetInkThickness(AnnotationController.CurrentThickness);
				SetPen(AnnotationController.CurrentColor);
				(Resources["ShowStoryBoard"] as Storyboard)?.Begin();
			}
		}

		public void HiddenWindow()
		{
			if (IsShowing)
			{
				IsShowing = false;
				// 通过 LowMaskButton 关闭时不经过 InnerBoard_Click，这里同步工具栏归属。
				ViewHelper.BottomArea.OnInnerBoardShowingChanged(false);
				ViewHelper.BottomArea.InnerBoardCheckBox.IsChecked = false;
				ViewHelper.MainWindow.LowMaskButton.Visibility = Visibility.Collapsed;
				(Resources["HiddenStoryBoard"] as Storyboard)?.Begin();
			}
		}

		public void HiddenAndClearInkCanvas()
		{
			HiddenWindow();
			ClearInkCanvas();
		}

		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			if (_currentIndex > 1)
			{
				_currentIndex--;
				// Board.Slides 天然保留逐页笔迹，不再需要手动 Clone / 回填 StrokeCollection。
				_board.GoToSlide(_currentIndex - 1);
				UpdateIndex();
				FridayReporter.ReportEventImmediately(EventID.InnerBoardPreviousPage);
			}
		}

		private void Next_OnClick(object sender, RoutedEventArgs e)
		{
			if (_currentIndex + 1 < MaxSlides)
			{
				FridayReporter.ReportEventImmediately((_currentIndex == _totalIndex) ? EventID.InnerBoardAddPage : EventID.InnerBoardNextPage);
				_currentIndex++;
				_totalIndex = Math.Max(_currentIndex, _totalIndex);
				// GoToSlide 会在页数不足时自动插入新 Slide（背景透明，白底由 Board 提供）。
				_board.GoToSlide(_currentIndex - 1);
				UpdateIndex();
			}
		}

		private void UpdateIndex()
		{
			Index.Text = _currentIndex + "/" + _totalIndex;
			Add.Visibility = ((_currentIndex != _totalIndex) ? Visibility.Collapsed : Visibility.Visible);
			Next.Visibility = ((_currentIndex == _totalIndex) ? Visibility.Collapsed : Visibility.Visible);
		}

		private void UpdateState()
		{
			// 每一次进入板中板一律重置为画笔（与 ShowWindow 一致）。
			// 原实现会按 Office 指针类型在动画结束后切到橡皮 / 选择，导致「进入后不是笔」；
			// 自实现批注下 Office 指针为 None、PointerColor 不可信，统一用 AnnotationController 真值源。
			SetPen(AnnotationController.CurrentColor);
		}

		private void ShowStoryBoard_OnCompleted(object sender, EventArgs e)
		{
			Transform renderTransform = RootGrid.RenderTransform;
			RootGrid.RenderTransform = null;
			RootGrid.RenderTransform = renderTransform;
			UpdateState();
		}

		private void HiddenStoryBoard_OnCompleted(object sender, EventArgs e)
		{
			if (ViewHelper.MainWindow.IsExiting)
			{
				PptOperator.Instance.Exit();
			}
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			Width = 1.0;
			Height = 1.0;
			DispatcherTask.Start(100.0, Hide);
		}

		public void ChangeWindowRect()
		{
			Left = ViewHelper.MainWindow.Left;
			Top = ViewHelper.MainWindow.Top;
			Width = ViewHelper.MainWindow.Width;
			Height = ViewHelper.MainWindow.Height;
		}

	}
}
