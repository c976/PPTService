using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	public partial class InnerBoardWindow : Window, IComponentConnector
	{
		private int _totalIndex = 1;

		private int _currentIndex = 1;

		private static readonly int MaxSlides = 100;

		private static readonly int WindowHeight = 2100;

		private StrokeCollection[] _strokeCollection;

		public bool IsShowing { get; set; }

		public InnerBoardWindow()
		{
			InitializeComponent();
			_strokeCollection = new StrokeCollection[MaxSlides];
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
			InkCanvas.DefaultDrawingAttributes.Width = 3.0;
			InkCanvas.DefaultDrawingAttributes.Height = 3.0;
			InkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
			_strokeCollection = new StrokeCollection[MaxSlides];
		}

		public void ClearInkCanvas()
		{
			InkCanvas.Strokes.Clear();
			_strokeCollection = new StrokeCollection[MaxSlides];
			_totalIndex = 1;
			_currentIndex = 1;
			UpdateIndex();
		}

		public void SetPenToInkMode()
		{
			InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
		}

		public void SetPen(Color color)
		{
			InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
			InkCanvas.DefaultDrawingAttributes.Color = color;
		}

		public void SetRubber()
		{
			InkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
			InkCanvas.EraserShape = new EllipseStylusShape(30.0, 30.0);
		}

		public void ShowWindow()
		{
			if (!IsShowing)
			{
				IsShowing = true;
				Show();
				ChangeWindowRect();
				InkCanvas.DefaultDrawingAttributes.Width = ((Height > (double)WindowHeight) ? 6 : 3);
				InkCanvas.DefaultDrawingAttributes.Height = ((Height > (double)WindowHeight) ? 6 : 3);
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
				_strokeCollection[_currentIndex] = InkCanvas.Strokes.Clone();
				InkCanvas.Strokes = _strokeCollection[--_currentIndex] ?? new StrokeCollection();
				UpdateIndex();
				FridayReporter.ReportEventImmediately(EventID.InnerBoardPreviousPage);
			}
		}

		private void Next_OnClick(object sender, RoutedEventArgs e)
		{
			if (_currentIndex + 1 < MaxSlides)
			{
				FridayReporter.ReportEventImmediately((_currentIndex == _totalIndex) ? EventID.InnerBoardAddPage : EventID.InnerBoardNextPage);
				_strokeCollection[_currentIndex] = InkCanvas.Strokes.Clone();
				InkCanvas.Strokes = _strokeCollection[++_currentIndex] ?? new StrokeCollection();
				_totalIndex = Math.Max(_currentIndex, _totalIndex);
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
				// 自实现批注：Office 指针类型为 None、PptOperator.PointerColor 不可信（读到黑色），
				// 板中板直接同步 AnnotationController 的当前笔色，避免「一开板中板笔就被强制黑色」。
				switch (PptOperator.Instance.PointerType)
				{
				case PpSlideShowPointerType.ppSlideShowPointerEraser:
					SetRubber();
					break;
				default:
					SetPen(AnnotationController.CurrentColor);
					break;
				}
				return;
			}
			// 以下为原 Office 批注路径，保持不变。
			switch (PptOperator.Instance.PointerType)
			{
			case PpSlideShowPointerType.ppSlideShowPointerPen:
				SetPen(PptOperator.Instance.PointerColor);
				break;
			case PpSlideShowPointerType.ppSlideShowPointerEraser:
				InkCanvas.DefaultDrawingAttributes.Color = PptOperator.Instance.PointerColor;
				SetRubber();
				break;
			default:
				ViewHelper.BottomArea.PenClick();
				SetPen(PptOperator.Instance.PointerColor);
				break;
			}
		}

		public void SetNone()
		{
			InkCanvas.EditingMode = InkCanvasEditingMode.None;
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
