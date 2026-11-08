using System;
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

		private void Recommendation_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.BottomRegionRecommendation);
			RecommendationWindow recommendationWindow = new RecommendationWindow
			{
				Owner = ViewHelper.MainWindow,
				Width = ViewHelper.MainWindow.Width,
				Height = ViewHelper.MainWindow.Height
			};
			ViewHelper.StopMainWindowUpdateTimerAndMouseGesture();
			recommendationWindow.ShowDialog();
			ViewHelper.BeginMainWindowUpdateTimerAndMouseGesture();
		}

	}
}
