using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	/// <summary>
	/// 板中板：与主批注层共用 PPTService.Ink 的新白板引擎（AnnotationBoard），
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

		private static readonly int WindowHeight = 2100;

		/// <summary>板中板自己的白板实例（Board 是独立 UIElement，不能与 MainWindow 共用）。</summary>
		private readonly AnnotationBoard _board;

		public bool IsShowing { get; set; }

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
		}

		private void InnerBoardWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Top = 0.0;
			_board.SetInkThickness(3.0);
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
			UpdateIndex();
		}

		/// <summary>切到画笔模式（沿用当前笔色）。</summary>
		public void SetPenToInkMode()
		{
			_board.GotoInk();
		}

		/// <summary>设置笔色并切到画笔模式。</summary>
		public void SetPen(Color color)
		{
			_board.SetInkColor(color);
			_board.GotoInk();
		}

		/// <summary>手势橡皮（拖动擦除）。</summary>
		public void SetRubber()
		{
			_board.GotoEraser();
		}

		/// <summary>鼠标 / 选择模式：白板不再接收笔迹输入。</summary>
		public void SetNone()
		{
			_board.GotoSelect();
		}

		public void ShowWindow()
		{
			if (!IsShowing)
			{
				IsShowing = true;
				Show();
				ChangeWindowRect();
				// 原版按窗口高度放大笔宽，这里映射到新引擎的 InkBoardSettings.InkThickness。
				_board.SetInkThickness(Height > (double)WindowHeight ? 6.0 : 3.0);
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
			if (UserConfigManager.Load().UseSelfAnnotation)
			{
				// 自实现批注：Office 指针类型为 None、PptOperator.PointerColor 不可信，
				// 统一取 AnnotationController.CurrentColor；且进入板中板默认切到笔模式。
				SetPen(AnnotationController.CurrentColor);
				return;
			}
			// 以下为原 Office 批注路径，模式 / 颜色仍按 Office 指针状态，只是落到新白板引擎。
			switch (PptOperator.Instance.PointerType)
			{
			case PpSlideShowPointerType.ppSlideShowPointerPen:
				SetPen(PptOperator.Instance.PointerColor);
				break;
			case PpSlideShowPointerType.ppSlideShowPointerEraser:
				SetRubber();
				break;
			default:
				ViewHelper.BottomArea.PenClick();
				SetPen(PptOperator.Instance.PointerColor);
				break;
			}
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
