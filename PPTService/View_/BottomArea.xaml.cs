using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	public partial class BottomArea : UserControl, IComponentConnector
	{
		public static readonly DependencyProperty PenBrushProperty = DependencyProperty.Register("PenBrush", typeof(Brush), typeof(BottomArea), new PropertyMetadata(Brushes.Red));

		public bool IsInnerBoardWindowShowing { get; set; }

		/// <summary>
		/// 自实现批注是否接管「主批注层」的工具栏按钮：板中板未打开时为真。
		/// 板中板打开时按钮改由 IsInnerBoardSelfAnnotation 分支驱动板中板自己的
		/// AnnotationBoard（同一套新白板引擎，只是目标 Board 不同）。
		/// </summary>
		public bool IsSelfAnnotationActive
		{
			get { return UserConfigManager.Load().UseSelfAnnotation && !IsInnerBoardWindowShowing; }
		}

		/// <summary>
		/// 板中板打开且使用自实现批注：按钮只驱动板中板新白板，
		/// 不再触碰 Office 指针（旧 InkCanvas 时代的 SetPptPointerType / SetPointerColor）。
		/// </summary>
		private bool IsInnerBoardSelfAnnotation
		{
			get { return UserConfigManager.Load().UseSelfAnnotation && IsInnerBoardWindowShowing; }
		}

		private AnnotationController.Mode _modeBeforeInnerBoard;

		/// <summary>
		/// 板中板显示/隐藏时同步工具栏按钮归属与批注窗口模式。
		/// 板中板打开期间主批注层设为鼠标穿透，避免抢走输入；但工具栏视觉上默认切到「画笔」
		/// 并同步当前笔色，让用户在工具栏看到当前颜色（进入板中板即默认画笔模式）。
		/// 关闭后恢复打开前的笔/橡皮/鼠标模式与工具栏视觉。
		/// </summary>
		public void OnInnerBoardShowingChanged(bool showing)
		{
			if (IsInnerBoardWindowShowing == showing)
			{
				return;
			}
			IsInnerBoardWindowShowing = showing;
			// 切换板中板时收起笔设置 / 滑动清屏弹窗，避免残留遮挡。
			EraserSliderPopup.IsOpen = false;
			ColorDiskPopup.IsOpen = false;
			if (!UserConfigManager.Load().UseSelfAnnotation)
			{
				return;
			}
			if (showing)
			{
				_modeBeforeInnerBoard = AnnotationController.CurrentMode;
				// 主批注层穿透，避免与板中板抢输入。
				AnnotationController.SetMode(AnnotationController.Mode.Mouse);
				// 工具栏默认切到画笔并同步当前笔色（进入板中板即画笔模式）。
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerPen);
				PenBrush = new SolidColorBrush(AnnotationController.CurrentColor);
			}
			else
			{
				AnnotationController.SetMode(_modeBeforeInnerBoard);
				// 恢复打开前的工具栏视觉状态。
				UpdateButtonState(ModeToPointerType(_modeBeforeInnerBoard));
				// 笔色同步回主批注层的当前颜色（板中板期间改的是它自己的笔色，不应污染工具栏）。
				PenBrush = new SolidColorBrush(AnnotationController.CurrentColor);
			}
		}

		private static PpSlideShowPointerType ModeToPointerType(AnnotationController.Mode mode)
		{
			switch (mode)
			{
			case AnnotationController.Mode.Pen:
				return PpSlideShowPointerType.ppSlideShowPointerPen;
			case AnnotationController.Mode.Eraser:
				return PpSlideShowPointerType.ppSlideShowPointerEraser;
			default:
				return PpSlideShowPointerType.ppSlideShowPointerNone;
			}
		}

		public bool IsNeedStopMouseLineTrack
		{
			get
			{
				if (RubberCheckBox.IsChecked != true && InnerBoardCheckBox.IsChecked != true)
				{
					return SelectCheckBox.IsChecked == false;
				}
				return true;
			}
		}

		public Brush PenBrush
		{
			get
			{
				return (Brush)GetValue(PenBrushProperty);
			}
			set
			{
				SetValue(PenBrushProperty, value);
			}
		}

		public BottomArea()
		{
			InitializeComponent();
			DataContext = this;
		}

		public void InitializeBottomArea()
		{
			// 每场放映开始都回到「鼠标」态：既保证工具栏视觉与 AnnotationController
			// 的模式一致，也避免上一场遗留的笔/橡皮选中态。
			UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerNone);
		}

		private void Select_OnClick(object sender, RoutedEventArgs e)
		{
			EraserSliderPopup.IsOpen = false;
			if (IsSelfAnnotationActive)
			{
				// 鼠标模式：批注窗口穿透，点击落到 PPT。
				// 不走 SetPptPointerType，避免把放映窗口提到前台而盖住批注层。
				AnnotationController.SetMode(AnnotationController.Mode.Mouse);
				PptOperator.Instance.PointerType = PpSlideShowPointerType.ppSlideShowPointerNone;
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerNone);
				FridayReporter.ReportEventImmediately(EventID.BottomRegionSelect);
				return;
			}
			if (IsInnerBoardSelfAnnotation)
			{
				// 板中板走新白板引擎：切到选择态，不再触碰 Office 指针（旧 InkCanvas 路径）。
				ViewHelper.InnerBoardWindow.SetNone();
				RubberCheckBox.IsChecked = false;
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerNone);
				FridayReporter.ReportEventImmediately(EventID.BottomRegionSelect);
				return;
			}
			if (IsInnerBoardWindowShowing)
			{
				ViewHelper.InnerBoardWindow.SetNone();
				RubberCheckBox.IsChecked = false;
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerNone);
			}
			SetPptPointerType(PpSlideShowPointerType.ppSlideShowPointerNone);
			FridayReporter.ReportEventImmediately(EventID.BottomRegionSelect);
		}

		private void Pen_OnClick(object sender, RoutedEventArgs e)
		{
			EraserSliderPopup.IsOpen = false;
			if (IsInnerBoardSelfAnnotation)
			{
				// 板中板：切到新白板引擎的画笔并同步当前颜色，工具栏视觉保持「笔」选中，
				// 不走 PenClick 的 Office 指针分支（自实现批注下 Office 指针必须保持 None）。
				ViewHelper.InnerBoardWindow.SetPen(AnnotationController.CurrentColor);
				SelectCheckBox.IsChecked = false;
				RubberCheckBox.IsChecked = false;
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerPen);
				ColorDiskPopup.IsOpen = true;
				FridayReporter.ReportEventImmediately(EventID.BottomRegionPen);
				return;
			}
			PenClick();
			if (IsInnerBoardWindowShowing)
			{
				ViewHelper.InnerBoardWindow.SetPenToInkMode();
			}
		}

		private void Rubber_OnClick(object sender, RoutedEventArgs e)
		{
			if (IsInnerBoardSelfAnnotation)
			{
				// 板中板：走新白板引擎的手势橡皮，不触碰 Office 指针。
				// 橡皮已选中时再点一次 → 弹出「滑动清屏」滑杆（移植桌面批注）。
				bool eraserAlreadyActive = ViewHelper.InnerBoardWindow.IsEraserActive;
				SelectCheckBox.IsChecked = false;
				RubberCheckBox.IsChecked = true;
				ViewHelper.InnerBoardWindow.SetRubber();
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerEraser);
				if (eraserAlreadyActive)
				{
					ShowEraserSliderPopup();
				}
				else
				{
					EraserSliderPopup.IsOpen = false;
				}
				FridayReporter.ReportEventImmediately(EventID.BottomRegionRubber);
				return;
			}
			if (IsSelfAnnotationActive)
			{
				// 手势橡皮：拖动擦除经过的笔迹；橡皮已选中时再点一次弹出
				// 「滑动清屏」滑杆，滑到底才清当前页（对应桌面批注 DesktopEraserPopupBorder）。
				bool eraserAlreadyActive = AnnotationController.IsEnabled &&
					AnnotationController.CurrentMode == AnnotationController.Mode.Eraser;
				AnnotationController.SetMode(AnnotationController.Mode.Eraser);
				SelectCheckBox.IsChecked = false;
				RubberCheckBox.IsChecked = true;
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerEraser);
				if (eraserAlreadyActive)
				{
					ShowEraserSliderPopup();
				}
				else
				{
					EraserSliderPopup.IsOpen = false;
				}
				FridayReporter.ReportEventImmediately(EventID.BottomRegionRubber);
				return;
			}
			if (IsInnerBoardWindowShowing)
			{
				SelectCheckBox.IsChecked = false;
				RubberCheckBox.IsChecked = true;
				ViewHelper.InnerBoardWindow.SetRubber();
				SetPptPointerType(PpSlideShowPointerType.ppSlideShowPointerEraser);
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerEraser);
			}
			else
			{
				SetPptPointerType(PpSlideShowPointerType.ppSlideShowPointerEraser);
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			}
			FridayReporter.ReportEventImmediately(EventID.BottomRegionRubber);
		}

		private void Magnifier_Click(object sender, RoutedEventArgs e)
		{
			PlayHiddenAllAreaStoryBoard();
			FridayReporter.ReportEventImmediately(EventID.BottomRegionMagnifier);
		}

		private void InnerBoard_Click(object sender, RoutedEventArgs e)
		{
			if (InnerBoardCheckBox.IsChecked == true)
			{
				OnInnerBoardShowingChanged(true);
				SelectCheckBox.IsChecked = false;
				ViewHelper.InnerBoardWindow.Left = ViewHelper.MainWindow.Left;
				ViewHelper.InnerBoardWindow.Top = ViewHelper.MainWindow.Top;
				ViewHelper.InnerBoardWindow.Width = ViewHelper.MainWindow.Width;
				ViewHelper.InnerBoardWindow.Height = ViewHelper.MainWindow.Height;
				ViewHelper.InnerBoardWindow.ShowWindow();
				ViewHelper.MainWindow.LowMaskButton.Visibility = Visibility.Visible;
				FridayReporter.ReportEventImmediately(EventID.BottomRegionOpenInnerBoard);
			}
			else
			{
				OnInnerBoardShowingChanged(false);
				ViewHelper.InnerBoardWindow.HiddenWindow();
				ViewHelper.MainWindow.LowMaskButton.Visibility = Visibility.Collapsed;
				FridayReporter.ReportEventImmediately(EventID.BottomRegionCloseInnerBoard);
			}
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			ViewHelper.MainWindow.IsExiting = true;
			bool isShowing = ViewHelper.InnerBoardWindow.IsShowing;
			ViewHelper.InnerBoardWindow.HiddenAndClearInkCanvas();
			if (!isShowing)
			{
				PptOperator.Instance.Exit();
			}
			FridayReporter.ReportEventImmediately(EventID.BottomRegionExit);
			DispatcherTask.Start(3000.0, AutoUpdater.Check);
		}

		private void RadioButton_OnClick(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = e.OriginalSource as RadioButton;
			if (radioButton != null)
			{
				SolidColorBrush brush = radioButton.Background as SolidColorBrush;
				if (UserConfigManager.Load().UseSelfAnnotation)
				{
					// 自实现批注（含板中板打开期间）：统一走 AnnotationController 作为颜色真值源，
					// 不走 SetPenColor → PptOperator.SetPointerColor 的 Office COM 路径
					// （自实现批注下 Office 指针必须保持 None）。
					if (brush != null)
					{
						AnnotationController.SetColor(brush.Color);
						if (IsInnerBoardWindowShowing)
						{
							// 板中板使用新白板引擎，同步它的笔色并保持在画笔模式。
							ViewHelper.InnerBoardWindow.SetPen(brush.Color);
						}
						// 立即刷新工具栏笔图标颜色，避免被 UpdateButtonState 从
						// PptOperator.PointerColor（自实现批注下不可信，常为黑）覆盖。
						PenBrush = new SolidColorBrush(brush.Color);
					}
					ColorDiskPopup.IsOpen = false;
					return;
				}
				SetPenColor(radioButton.Background.ToString());
				ColorDiskPopup.IsOpen = false;
				if (!IsInnerBoardWindowShowing)
				{
					WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				}
			}
		}

		/// <summary>
		/// 笔迹粗细选择（移植自桌面批注 DesktopPenPopupBorder 的 ThicknessGrid）。
		/// 粗细由 AnnotationController 统一保存，同时同步主批注层与板中板。
		/// </summary>
		private void ThicknessRadioButton_OnClick(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = e.OriginalSource as RadioButton;
			if (radioButton == null || !(radioButton.Tag is double))
			{
				return;
			}
			double thickness = (double)radioButton.Tag;
			// 无条件记录真值（纯静态状态，无副作用），保证切换到自实现批注后
			// 新建 Board 时能把用户选过的粗细应用上；Office 批注的笔宽由 Office 自身控制。
			AnnotationController.SetThickness(thickness);
			if (UserConfigManager.Load().UseSelfAnnotation && IsInnerBoardWindowShowing)
			{
				ViewHelper.InnerBoardWindow.SetPenThickness(thickness);
			}
		}

		/// <summary>打开「滑动清屏」滑杆，并关闭其它工具栏弹窗。</summary>
		private void ShowEraserSliderPopup()
		{
			ColorDiskPopup.IsOpen = false;
			// 上一次回弹动画可能仍在持有 Value（HoldEnd），先解除再复位。
			EraserSlide.BeginAnimation(RangeBase.ValueProperty, null);
			EraserSlide.Value = 0.0;
			EraserSliderPopup.IsOpen = true;
		}

		private void EraserSliderPopup_OnOpened(object sender, EventArgs e)
		{
			EraserSlide.Value = 0.0;
		}

		private void EraserSlide_DragStarted(object sender, DragStartedEventArgs e)
		{
			// 停掉上一次的回弹动画，避免拖动时被动画拉回。
			EraserSlide.BeginAnimation(RangeBase.ValueProperty, null);
		}

		/// <summary>
		/// 松手后滑杆回弹；滑到阈值以上才清「当前页」笔迹。
		/// 移植自桌面批注 DesktopEraserPopupBorder.Tracker_DragCompleted（阈值 0.8）。
		/// </summary>
		private void EraserSlide_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			double value = EraserSlide.Value;
			DoubleAnimation snapBack = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(300.0))
			{
				EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
			};
			EraserSlide.BeginAnimation(RangeBase.ValueProperty, snapBack);
			if (value < PenSettings.EraserSlideThreshold)
			{
				// 未滑到底：只回弹，弹窗保持打开，用户可继续尝试。
				return;
			}
			EraserSliderPopup.IsOpen = false;
			if (IsInnerBoardSelfAnnotation)
			{
				ViewHelper.InnerBoardWindow.ClearCurrentPage();
			}
			else if (UserConfigManager.Load().UseSelfAnnotation)
			{
				AnnotationController.ClearCurrentPage();
			}
		}

		private void ColorDiskPopup_OnOpened(object sender, EventArgs e)
		{
			// 自实现批注下 PptOperator.PointerColor 不可信（Office 指针为 None，常读到黑），
			// 颜色盘勾选状态一律取 AnnotationController 的真值源（板中板同样适用）。
			// PptOperator.Instance 由 App.OnStartup 赋值，放映未开始 / 初始化前可能为 null。
			Color pointerColor = UserConfigManager.Load().UseSelfAnnotation || PptOperator.Instance == null
				? AnnotationController.CurrentColor
				: PptOperator.Instance.PointerColor;
			foreach (object child in ColorDiskGrid.Children)
			{
				RadioButton radioButton = child as RadioButton;
				if (radioButton == null)
				{
					continue;
				}
				SolidColorBrush brush = radioButton.Background as SolidColorBrush;
				radioButton.IsChecked = brush != null && brush.Color == pointerColor;
			}
			foreach (object child in ThicknessGrid.Children)
			{
				RadioButton radioButton = child as RadioButton;
				if (radioButton == null || !(radioButton.Tag is double))
				{
					continue;
				}
				radioButton.IsChecked = Math.Abs((double)radioButton.Tag - AnnotationController.CurrentThickness) < 0.0001;
			}
			EraserSliderPopup.IsOpen = false;
			(Resources["ShowColorDiskStoryBoard"] as Storyboard)?.Begin();
		}

		public void PenClick()
		{
			if (IsSelfAnnotationActive)
			{
				// 画笔模式：用选中颜色在覆盖层上绘制（启用压感）
				AnnotationController.SetMode(AnnotationController.Mode.Pen);
				SelectCheckBox.IsChecked = false;
				RubberCheckBox.IsChecked = false;
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerPen);
				ColorDiskPopup.IsOpen = true;
				FridayReporter.ReportEventImmediately(EventID.BottomRegionPen);
				return;
			}
			SelectCheckBox.IsChecked = false;
			if (SelectedPenImage.Visibility == Visibility.Collapsed)
			{
				PptOperator.Instance.PointerType = PpSlideShowPointerType.ppSlideShowPointerPen;
				UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerPen);
				RubberCheckBox.IsChecked = false;
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				FridayReporter.ReportEventImmediately(EventID.BottomRegionPen);
			}
			else
			{
				ColorDiskPopup.IsOpen = true;
				FridayReporter.ReportEventImmediately(EventID.BottomRegionColorDisk);
			}
		}

		private void SetPenColor(string color)
		{
			byte r = (byte)Convert.ToInt32(color.Substring(3, 2), 16);
			byte g = (byte)Convert.ToInt32(color.Substring(5, 2), 16);
			byte b = (byte)Convert.ToInt32(color.Substring(7, 2), 16);
			int pointerColor = Convert.ToInt32(color.Substring(7, 2) + color.Substring(5, 2) + color.Substring(3, 2), 16);
			PptOperator.Instance.SetPointerColor(pointerColor);
			PptOperator.Instance.SetPointerColor(pointerColor);
			ViewHelper.InnerBoardWindow.SetPen(Color.FromRgb(r, g, b));
			PenBrush = new SolidColorBrush(Color.FromRgb(r, g, b));
			PostPenColorEvent(r, g, b);
		}

		private static void PostPenColorEvent(byte r, byte g, byte b)
		{
			if (r == byte.MaxValue && g == 0 && b == 0)
			{
				FridayReporter.ReportEventImmediately(EventID.BottomRegionRedPen);
			}
			if (r == 0 && g == 133 && b == 205)
			{
				FridayReporter.ReportEventImmediately(EventID.BottomRegionBluePen);
			}
			if (r == 0 && g == 0 && b == 0)
			{
				FridayReporter.ReportEventImmediately(EventID.BottomRegionBlackPen);
			}
		}

		private void SetPptPointerType(PpSlideShowPointerType type)
		{
			PptOperator.Instance.PointerType = type;
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
		}

		public void UpdateButtonState(PpSlideShowPointerType type)
		{
			switch (type)
			{
			case PpSlideShowPointerType.ppSlideShowPointerPen:
				SelectCheckBox.IsChecked = false;
				RubberCheckBox.IsChecked = false;
				PenImage.Visibility = Visibility.Collapsed;
				SelectedPenImage.Visibility = Visibility.Visible;
				// 笔图标颜色：自实现批注（含板中板）用 AnnotationController 真值源，
				// 否则 UpdateButtonState 会把调色板选中的颜色覆盖成 Office 的黑色。
				PenBrush = new SolidColorBrush(UserConfigManager.Load().UseSelfAnnotation
					? AnnotationController.CurrentColor
					: PptOperator.Instance.PointerColor);
				if (PptAnalyzer.CurrentAnalyzeRate != AnalyzeRate.Highest)
				{
					PptAnalyzer.StopAnalyzeAllAsyn();
				}
				break;
			case PpSlideShowPointerType.ppSlideShowPointerEraser:
				SelectCheckBox.IsChecked = false;
				RubberCheckBox.IsChecked = true;
				PenImage.Visibility = Visibility.Visible;
				SelectedPenImage.Visibility = Visibility.Collapsed;
				if (PptAnalyzer.CurrentAnalyzeRate != AnalyzeRate.Highest)
				{
					PptAnalyzer.StopAnalyzeAllAsyn();
				}
				break;
			default:
				SelectCheckBox.IsChecked = true;
				RubberCheckBox.IsChecked = false;
				PenImage.Visibility = Visibility.Visible;
				SelectedPenImage.Visibility = Visibility.Collapsed;
				break;
			}
		}

		public void PlayHiddenAllAreaStoryBoard()
		{
			ViewHelper.LeftArea.PlayHiddenOrShowLeftAreaStoryBoard(true);
			ViewHelper.RightArea.PlayHiddenOrShowLeftAreaStoryBoard(true);
			ViewHelper.LeftBottomArea.PlayHiddenOrShowLeftAreaStoryBoard(true);
			ViewHelper.RightBottomArea.PlayHiddenOrShowLeftAreaStoryBoard(true);
			double value = ActualHeight + ViewHelper.MainWindow.RootGrid.Margin.Bottom + 3.0;
			DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
			DoubleKeyFrame keyFrame = new EasingDoubleKeyFrame(value, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300.0)));
			doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame);
			Storyboard.SetTarget(doubleAnimationUsingKeyFrames, RootGrid);
			Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
			Storyboard storyboard = new Storyboard();
			storyboard.Children = new TimelineCollection { doubleAnimationUsingKeyFrames };
			storyboard.Completed += (object s, EventArgs e) =>
			{
				SyntacticSugar.TryCatchAction(() =>
				{
					Visibility = Visibility.Collapsed;
					ViewHelper.Magnifier = new Magnifier
					{
						Owner = ViewHelper.MainWindow
					};
					ViewHelper.Magnifier.ShowDialog();
					ViewHelper.Magnifier = null;
					PlayShowLeftAreaStoryBoard();
					ViewHelper.MainWindow.UpdatePageIndex();
					ViewHelper.LeftArea.PlayHiddenOrShowLeftAreaStoryBoard(false);
					ViewHelper.RightArea.PlayHiddenOrShowLeftAreaStoryBoard(false);
					ViewHelper.LeftBottomArea.PlayHiddenOrShowLeftAreaStoryBoard(false);
					ViewHelper.RightBottomArea.PlayHiddenOrShowLeftAreaStoryBoard(false);
				});
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			};
			storyboard.Begin();
		}

		public void PlayShowLeftAreaStoryBoard()
		{
			Visibility = Visibility.Visible;
			DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
			DoubleKeyFrame keyFrame = new EasingDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300.0)));
			doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame);
			Storyboard.SetTarget(doubleAnimationUsingKeyFrames, RootGrid);
			Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
			Storyboard storyboard = new Storyboard();
			storyboard.Children = new TimelineCollection { doubleAnimationUsingKeyFrames };
			storyboard.Begin();
		}

		private void Share_OnClick(object sender, RoutedEventArgs e)
		{
			// 模拟放映没有真实 PPT 文件，避免触发二维码上传逻辑。
			if (DebugMode.IsEnabled)
			{
				return;
			}
			PptSharer.Share(PptOperator.Instance.PptFilePath);
		}

		private void Settings_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.BottomRegionRecommendation);
			// 原「关于/版本」按钮改为打开设置窗口（含版本信息显示）
			SettingsWindow settingsWindow = new SettingsWindow
			{
				Owner = ViewHelper.MainWindow
			};
			settingsWindow.ShowDialog();
		}

		/// <summary>
		/// 依据用户配置重排/显隐工具栏中央功能按钮（选择/画笔/橡皮/放大镜/板中板），
		/// 以及左侧「创建二维码（分享）」按钮的显隐。中心区每列固定 50，宽度随可见按钮数自适应，
		/// 避免隐藏按钮后剩余图标（如放大镜）被拉宽变形。
		/// </summary>
		public void ApplyToolbarConfig(UserConfig config)
		{
			if (config == null)
			{
				return;
			}
			// 左侧「设置 + 创建二维码（分享）」按钮组：
			// 设置按钮常驻；二维码按钮按 ShowQrCode 显隐。与中央工具栏一致——
			// 动态重建 ShareGrid 列定义并据可见按钮数自适应宽度，避免隐藏二维码后
			// 整个左侧被隐藏（设置按钮也消失）或留下空白。
			List<UIElement> leftVisible = new List<UIElement>();
			if (SettingsButton != null)
			{
				leftVisible.Add(SettingsButton);
			}
			if (ShareButton != null)
			{
				if (config.ShowQrCode)
				{
					leftVisible.Add(ShareButton);
				}
				else
				{
					ShareButton.Visibility = Visibility.Collapsed;
				}
			}
			if (ShareGrid != null)
			{
				ShareGrid.ColumnDefinitions.Clear();
				for (int i = 0; i < leftVisible.Count; i++)
				{
					ShareGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50.0) });
					Grid.SetColumn(leftVisible[i], i);
					leftVisible[i].Visibility = Visibility.Visible;
				}
			}
			if (LeftBackgroundBorder != null)
			{
				LeftBackgroundBorder.SetValue(Grid.ColumnSpanProperty, leftVisible.Count == 0 ? 1 : leftVisible.Count);
			}
			if (config.ToolbarButtons == null)
			{
				return;
			}
			Dictionary<string, UIElement> map = new Dictionary<string, UIElement>
			{
				{ "Select", SelectCheckBox },
				{ "Pen", PenButton },
				{ "Rubber", RubberCheckBox },
				{ "Magnifier", MagnifierButton },
				{ "InnerBoard", InnerBoardCheckBox }
			};
		List<UIElement> visible = new List<UIElement>();
		UIElement element;
		foreach (ToolbarButtonSetting button in config.ToolbarButtons)
		{
			if (map.TryGetValue(button.Id, out element))
			{
				if (button.Visible)
				{
					visible.Add(element);
				}
				else
				{
					element.Visibility = Visibility.Collapsed;
				}
			}
		}
			BottomCenterButtonGrid.ColumnDefinitions.Clear();
			for (int i = 0; i < visible.Count; i++)
			{
				BottomCenterButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50.0) });
				Grid.SetColumn(visible[i], i);
				visible[i].Visibility = Visibility.Visible;
			}
			BackgroundBorder.SetValue(Grid.ColumnSpanProperty, visible.Count == 0 ? 1 : visible.Count);
		}

	}
}
