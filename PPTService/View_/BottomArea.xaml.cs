using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
			RubberCheckBox.IsChecked = false;
		}

		private void Select_OnClick(object sender, RoutedEventArgs e)
		{
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
			PenClick();
			if (IsInnerBoardWindowShowing)
			{
				ViewHelper.InnerBoardWindow.SetPenToInkMode();
			}
		}

		private void Rubber_OnClick(object sender, RoutedEventArgs e)
		{
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
				SetPenColor(radioButton.Background.ToString());
				ColorDiskPopup.IsOpen = false;
				if (!IsInnerBoardWindowShowing)
				{
					WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				}
			}
		}

		private void ColorDiskPopup_OnOpened(object sender, EventArgs e)
		{
			RedRadioButton.IsChecked = false;
			BlueRadioButton.IsChecked = false;
			BlackRadioButton.IsChecked = false;
			Color pointerColor = PptOperator.Instance.PointerColor;
			if (pointerColor == Colors.Red)
			{
				RedRadioButton.IsChecked = true;
			}
			if (pointerColor == Colors.Black)
			{
				BlackRadioButton.IsChecked = true;
			}
			if (pointerColor.R == 0 && pointerColor.G == 133 && pointerColor.B == 205)
			{
				BlueRadioButton.IsChecked = true;
			}
			(Resources["ShowColorDiskStoryBoard"] as Storyboard)?.Begin();
		}

		public void PenClick()
		{
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
				PenBrush = new SolidColorBrush(PptOperator.Instance.PointerColor);
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
