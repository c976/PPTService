using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Cvte.Windows;
using Microsoft.Office.Interop.PowerPoint;
using PPTService.Collection;

namespace PPTService
{
	public partial class MainWindow : Window, IComponentConnector
	{
		private Gesture _gesture;

		private bool _isAnalyzing;

		private int _currentPageIndex;

		private string _prePptFilePath;

		private static readonly int TimerSpan = 300;

		private static readonly int ReleaseTimerSpan = 6000;

		private readonly DispatcherTimer _releaseTimer;

		public static readonly DependencyProperty SlideItemsProperty = DependencyProperty.Register("SlideItems", typeof(ObservableCollection<SlideItem>), typeof(MainWindow), new PropertyMetadata((object)null));

		private bool IsTransparent => Opacity < 0.1;

		public bool IsExiting { get; set; }

		public DispatcherTimer UpdateTimer { get; set; }

		public bool IsNeedUpdateTimer { get; set; } = true;

		public bool IsLeftSlideManagerViewShowed { get; set; }

		public bool IsRightSlideManagerViewShowed { get; set; }

		public ObservableCollection<SlideItem> SlideItems
		{
			get
			{
				return (ObservableCollection<SlideItem>)GetValue(SlideItemsProperty);
			}
			set
			{
				SetValue(SlideItemsProperty, value);
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			UpdateTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(TimerSpan)
			};
			UpdateTimer.Tick += UpdateTimerTick;
			_releaseTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(ReleaseTimerSpan)
			};
			_releaseTimer.Tick += ReleaseTimerTick;
			Loaded += MainWindow_Loaded;
			PreviewKeyDown += MainWindow_KeyDown;
			SizeChanged += MainWindow_SizeChanged;
			LocationChanged += MainWindow_LocationChanged;
			ManipulationBoundaryFeedback += (object s, ManipulationBoundaryFeedbackEventArgs e) =>
			{
				e.Handled = true;
			};
			LeftSlideListBox.ManipulationBoundaryFeedback += (object s, ManipulationBoundaryFeedbackEventArgs e) =>
			{
				e.Handled = true;
			};
			RightSlideListBox.ManipulationBoundaryFeedback += (object s, ManipulationBoundaryFeedbackEventArgs e) =>
			{
				e.Handled = true;
			};
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (PptVersionManager.IsOffice2016OrLater)
			{
				LeftBottomButtonGrid.Children.Add(ViewHelper.LeftBottomArea);
				RightBottomButtonGrid.Children.Add(ViewHelper.RightBottomArea);
				LeftSlideManagerViewGrid.RenderTransformOrigin = new Point(0.5, 1.0);
				RightSlideManagerViewGrid.RenderTransformOrigin = new Point(0.5, 1.0);
				LeftSlideManagerViewGrid.VerticalAlignment = VerticalAlignment.Bottom;
				RightSlideManagerViewGrid.VerticalAlignment = VerticalAlignment.Bottom;
			}
			else
			{
				LeftCenterButtonGrid.Children.Add(ViewHelper.LeftArea);
				RightCenterButtonGrid.Children.Add(ViewHelper.RightArea);
				LeftSlideManagerViewGrid.RenderTransformOrigin = new Point(0.0, 0.5);
				RightSlideManagerViewGrid.RenderTransformOrigin = new Point(1.0, 0.5);
				LeftSlideManagerViewGrid.VerticalAlignment = VerticalAlignment.Center;
				RightSlideManagerViewGrid.VerticalAlignment = VerticalAlignment.Center;
			}
			BottomCenterButtonGrid.Children.Add(ViewHelper.BottomArea);
			Width = SystemParameters.PrimaryScreenWidth;
			Height = SystemParameters.PrimaryScreenHeight;
			SubscribeSelectionChangedEvent();
			_gesture = new Gesture();
			_gesture.ChangeGestureScope();
			WindowsApi.SetPopupParameter();
			PptOperator.Instance.ShowBegin += _pptOperatorShowBegin;
			PptOperator.Instance.ShowEnd += _pptOperatorShowEnd;
			PptOperator.Instance.NextSlide += PptOperatorNextSlide;
		}

		private void MainWindow_LocationChanged(object sender, EventArgs e)
		{
			ResetPreWindowRect();
		}

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			AdjustFontSizeAndStyle();
		}

		public void _pptOperatorShowBegin(object sender, EventArgs e)
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				WindowStateHelper.UpdateWindowRect(this);
				Task.Factory.StartNew(() =>
				{
					McuService.ForbiddenAndroidPen();
				});
				SyntacticSugar.CurrentDispatcherInvoke(() =>
				{
					_releaseTimer?.Stop();
					Show();
					Activate();
					AdjustFontSizeAndStyle();
					ViewHelper.BottomArea?.InitializeBottomArea();
					_currentPageIndex = PptOperator.Instance.CurrentShowPosition;
					SetPageIndex(_currentPageIndex);
					SetLeftRightOpacity();
					IsExiting = false;
					Opacity = 1.0;
				});
				SyntacticSugar.CurrentDispatcherBeginInvoke(AnalyzePpt);
			});
		}

		private void _pptOperatorShowEnd(object sender, EventArgs e)
		{
			ShowEnd();
		}

		private void PptOperatorNextSlide(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(() =>
			{
				if (!IsTransparent)
				{
					UpdatePageIndex();
				}
			});
		}

		private void HighMaskButton_Click(object sender, RoutedEventArgs e)
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				HighMaskButton.Visibility = Visibility.Collapsed;
				PlayHiddenLeftSlideManagerViewStoryboard();
				PlayHiddenRightSlideManagerViewStoryboard();
				IsLeftSlideManagerViewShowed = false;
				IsRightSlideManagerViewShowed = false;
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				FridayReporter.ReportEventImmediately(EventID.CenterRegionHiddenThumbnailListBox);
			});
		}

		private void LowMaskButton_Click(object sender, MouseButtonEventArgs e)
		{
			ViewHelper.InnerBoardWindow.HiddenWindow();
			FridayReporter.ReportEventImmediately(EventID.CenterRegionHiddenInnerBoard);
		}

		private void ShowLeftSlideManagerView_OnCompleted(object sender, EventArgs e)
		{
			IsLeftSlideManagerViewShowed = true;
			HighMaskButton.Visibility = Visibility.Visible;
			(Resources["ShowLeftCenterButtonGridStoryBoard"] as Storyboard)?.Begin();
			SyntacticSugar.CurrentDispatcherBeginInvoke(DispatcherPriority.Background, UpdatePageIndex);
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			PptAnalyzer.CurrentShowPosition = _currentPageIndex;
			PptAnalyzer.StartAnalyzeAllAsync(AnalyzeRate.Highest);
		}

		private void ShowRightSlideManagerView_OnCompleted(object sender, EventArgs e)
		{
			IsRightSlideManagerViewShowed = true;
			HighMaskButton.Visibility = Visibility.Visible;
			(Resources["ShowRightCenterButtonGridStoryBoard"] as Storyboard)?.Begin();
			SyntacticSugar.CurrentDispatcherBeginInvoke(DispatcherPriority.Background, UpdatePageIndex);
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			PptAnalyzer.CurrentShowPosition = _currentPageIndex;
			PptAnalyzer.StartAnalyzeAllAsync(AnalyzeRate.Highest);
		}

		private void HiddenLeftSlideManagerView_OnCompleted(object sender, EventArgs e)
		{
			IsLeftSlideManagerViewShowed = false;
			HighMaskButton.Visibility = Visibility.Collapsed;
			PlayHiddenLeftCenterButtonGridStoryboard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			PptAnalyzer.StopAnalyzeAllAsyn();
		}

		private void HiddenRightSlideManagerView_OnCompleted(object sender, EventArgs e)
		{
			IsRightSlideManagerViewShowed = false;
			HighMaskButton.Visibility = Visibility.Collapsed;
			PlayHiddenRightCenterButtonGridStoryboard();
			WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			PptAnalyzer.StopAnalyzeAllAsyn();
		}

		public void PlayHiddenLeftSlideManagerViewStoryboard()
		{
			if (IsLeftSlideManagerViewShowed)
			{
				PlayStoryboard(PptVersionManager.IsOffice2016OrLater ? "HiddenLeftBottomSlideManagerViewStoryboard" : "HiddenLeftSlideManagerViewStoryboard");
			}
		}

		public void PlayHiddenRightSlideManagerViewStoryboard()
		{
			if (IsRightSlideManagerViewShowed)
			{
				PlayStoryboard(PptVersionManager.IsOffice2016OrLater ? "HiddenRightBottomSlideManagerViewStoryboard" : "HiddenRightSlideManagerViewStoryboard");
			}
		}

		public void PlayHiddenLeftCenterButtonGridStoryboard()
		{
			(Resources["HiddenLeftCenterButtonGridStoryBoard"] as Storyboard)?.Begin();
		}

		public void PlayHiddenRightCenterButtonGridStoryboard()
		{
			(Resources["HiddenRightCenterButtonGridStoryBoard"] as Storyboard)?.Begin();
		}

		public void PlayStoryboard(string storyBoardName)
		{
			if (Width > 3800.0)
			{
				(Resources[storyBoardName + "3800"] as Storyboard)?.Begin();
			}
			if (Width > 1440.0 && Width <= 3800.0)
			{
				(Resources[storyBoardName + "1440"] as Storyboard)?.Begin();
			}
			if (Width > 0.0 && Width <= 1440.0)
			{
				(Resources[storyBoardName + "0"] as Storyboard)?.Begin();
			}
		}

		public void UpdatePageIndex()
		{
			if (_isAnalyzing)
			{
				return;
			}
			SyntacticSugar.TryCatchAction(() =>
			{
				_currentPageIndex = PptOperator.Instance.CurrentShowPosition;
				if (_currentPageIndex <= PptOperator.Instance.SlidesCount)
				{
					if (_currentPageIndex <= 0)
					{
						_currentPageIndex = 1;
					}
					SetPageIndex(_currentPageIndex);
					UpdateSlideListBoxIndex(_currentPageIndex);
				}
			});
		}

		private void UpdateButtonState()
		{
			PptOperator.Instance.UpdatePointerColor();
			ViewHelper.BottomArea.IsInnerBoardWindowShowing = ViewHelper.InnerBoardWindow.IsShowing;
			if (!ViewHelper.InnerBoardWindow.IsShowing)
			{
				ViewHelper.BottomArea.UpdateButtonState(PptOperator.Instance.PointerType);
			}
		}

		private void UpdateWindowOpacity()
		{
			if (!_isAnalyzing)
			{
				bool pptShowingState = WindowStateHelper.GetPptShowingState(this);
				if (IsTransparent & pptShowingState)
				{
					UpdatePageIndex();
				}
				Opacity = (pptShowingState ? 1 : 0);
			}
		}

		private void UpdateSlideListBoxIndex(int index)
		{
			CancelSubscribeSelectionChangedEvent();
			int num = index - 1;
			if (LeftSlideListBox.SelectedIndex != num)
			{
				LeftSlideListBox.SelectedIndex = num;
				LeftSlideListBox.ScrollIntoView(LeftSlideListBox.SelectedItem);
				PptAnalyzer.CurrentShowPosition = index;
			}
			if (RightSlideListBox.SelectedIndex != num)
			{
				RightSlideListBox.SelectedIndex = num;
				RightSlideListBox.ScrollIntoView(RightSlideListBox.SelectedItem);
			}
			SubscribeSelectionChangedEvent();
		}

		private void UpdateGestureAndMouseLineTrackState()
		{
			if (IsTransparent || ViewHelper.MainWindow.IsLeftSlideManagerViewShowed || ViewHelper.MainWindow.IsRightSlideManagerViewShowed || ViewHelper.BottomArea.IsNeedStopMouseLineTrack)
			{
				MouseHelper.Instance.Stop();
			}
			else
			{
				MouseHelper.Instance.Start();
			}
		}

		public void HiddenWindow()
		{
			Width = 1.0;
			Height = 1.0;
			WindowStateHelper.PreWindowRect.Bottom = 1;
			DispatcherTask.Start(100.0, Hide);
		}

		private void AnalyzePpt()
		{
			if (_isAnalyzing)
			{
				return;
			}
			_isAnalyzing = true;
			SyntacticSugar.TryCatchAction(() =>
			{
				string pptFilePath = PptOperator.Instance.PptFilePath;
				List<string> imageFiles;
				bool flag = PptAnalyzer.FindAlreadyAnalyzedFiles(pptFilePath, out imageFiles);
				if (((SlideItems != null && pptFilePath == _prePptFilePath) & flag) || PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowNamedSlideShow)
				{
					DoAfterAnalyzePpt();
				}
				else
				{
					if (flag)
					{
						SetListBoxItem(imageFiles);
						DoAfterAnalyzePpt();
					}
					else
					{
						LoadingScreen<LoadingView>.Current.Show(new Rect
						{
							X = Left,
							Y = Top,
							Width = Width,
							Height = Height
						});
						LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
						{
							loadingView.Start();
							loadingView.CancelAction = () =>
							{
								Restart.PPTService();
								Environment.Exit(0);
							};
						});
						LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
						{
							loadingView.Progress = 100;
						});
						InitListBoxItem();
						LoadingScreen<LoadingView>.Current.UsingLoadingThread((LoadingView loadingView) =>
						{
							loadingView.PushFrame();
							LoadingScreen<LoadingView>.Current.Hide();
							SyntacticSugar.CurrentDispatcherBeginInvoke(DoAfterAnalyzePpt);
						});
					}
					_prePptFilePath = pptFilePath;
				}
			});
		}

		private void ShowEnd()
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				SyntacticSugar.CurrentDispatcherBeginInvoke(() =>
				{
					if (ConfigHelper.Instance.IsNeedStart() || !IsTransparent)
					{
						Opacity = 0.0;
						UpdateTimer.Stop();
						MouseHelper.Instance.Stop();
						ViewHelper.BottomArea.UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerNone);
						ViewHelper.InnerBoardWindow.HiddenAndClearInkCanvas();
						PlayHiddenLeftSlideManagerViewStoryboard();
						PlayHiddenRightSlideManagerViewStoryboard();
						_releaseTimer.Stop();
						_releaseTimer.Start();
						IsExiting = false;
						Task.Factory.StartNew(() =>
						{
							McuService.OpenAndroidPen();
						});
						FridayReporter.ReportEventImmediately(EventID.PptShowEnd);
					}
				});
			});
		}

		private void ResetPreWindowRect()
		{
			WindowStateHelper.PreWindowRect = new WindowRect
			{
				Left = (int)Left,
				Top = (int)Top,
				Right = (int)(Left + Width),
				Bottom = (int)(Top + Height)
			};
		}

		private void AdjustFontSizeAndStyle()
		{
			double fontSize = 13.0;
			string key = "Style.ListBox1440";
			if (Width > 3800.0)
			{
				fontSize = ((PptOperator.Instance.SlidesCount >= 100) ? 20 : 30);
				key = "Style.ListBox3800";
			}
			if (Width > 1440.0 && Width <= 3800.0)
			{
				fontSize = ((PptOperator.Instance.SlidesCount >= 100) ? 11 : 13);
				key = "Style.ListBox1440";
			}
			if (Width > 0.0 && Width <= 1400.0)
			{
				fontSize = ((PptOperator.Instance.SlidesCount >= 100) ? 7 : 9);
				key = "Style.ListBox0";
			}
			ViewHelper.LeftArea.LeftSlideIndexTextBlock.FontSize = fontSize;
			ViewHelper.RightArea.RightSlideIndexTextBlock.FontSize = fontSize;
			ViewHelper.LeftBottomArea.LeftSlideIndexTextBlock.FontSize = fontSize;
			ViewHelper.RightBottomArea.RightSlideIndexTextBlock.FontSize = fontSize;
			LeftSlideListBox.Style = Resources[key] as Style;
			RightSlideListBox.Style = Resources[key] as Style;
		}

		private void DoAfterAnalyzePpt()
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				UpdateTimer.Start();
				_isAnalyzing = false;
				MouseHelper.Instance.Start();
				PlayHiddenLeftCenterButtonGridStoryboard();
				PlayHiddenRightCenterButtonGridStoryboard();
				FridayReporter.ReportEventImmediately(EventID.PptShowBegin);
				WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
			});
		}

		private void SubscribeSelectionChangedEvent()
		{
			LeftSlideListBox.SelectionChanged += SlideListBox_SelectionChanged;
			RightSlideListBox.SelectionChanged += SlideListBox_SelectionChanged;
		}

		private void CancelSubscribeSelectionChangedEvent()
		{
			LeftSlideListBox.SelectionChanged -= SlideListBox_SelectionChanged;
			RightSlideListBox.SelectionChanged -= SlideListBox_SelectionChanged;
		}

		private void SetLeftRightOpacity()
		{
			ViewHelper.LeftArea.Opacity = 1.0;
			ViewHelper.RightArea.Opacity = 1.0;
			ViewHelper.LeftArea.IsHitTestVisible = true;
			ViewHelper.RightArea.IsHitTestVisible = true;
			ViewHelper.LeftBottomArea.Opacity = 1.0;
			ViewHelper.RightBottomArea.Opacity = 1.0;
			ViewHelper.LeftBottomArea.IsHitTestVisible = true;
			ViewHelper.RightBottomArea.IsHitTestVisible = true;
		}

		private void SetPageIndex(int index)
		{
			if (index <= 0)
			{
				index = 1;
			}
			if (PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowSlideRange)
			{
				index += PptOperator.Instance.StartingSlide - 1;
			}
			string text = ((PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowNamedSlideShow) ? $"{index}" : $"{index}/{PptOperator.Instance.SlidesCount}");
			ViewHelper.LeftArea.LeftSlideIndexTextBlock.Text = text;
			ViewHelper.RightArea.RightSlideIndexTextBlock.Text = text;
			ViewHelper.LeftBottomArea.LeftSlideIndexTextBlock.Text = text;
			ViewHelper.RightBottomArea.RightSlideIndexTextBlock.Text = text;
		}

		private void InitListBoxItem()
		{
			string[] array = new string[PptOperator.Instance.SlidesCount];
			foreach (KeyValuePair<int, string> item in PptAnalyzer.AnalyzedIndexAndThumbnailFilePathDictionary)
			{
				array[item.Key - 1] = item.Value;
			}
			SetListBoxItem(array.ToList());
		}

		private void SetListBoxItem(List<string> files)
		{
			if (files == null || !files.Any())
			{
				return;
			}
			CancelSubscribeSelectionChangedEvent();
			SlideItems = new ObservableCollection<SlideItem>();
			if (PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowAll)
			{
				int num = 1;
				foreach (string file in files)
				{
					CreateSlideItem(num++, file);
				}
			}
			if (PptOperator.Instance.ShowRangeType == PpSlideShowRangeType.ppShowSlideRange)
			{
				for (int i = PptOperator.Instance.StartingSlide; i <= PptOperator.Instance.EndingSlide; i++)
				{
					CreateSlideItem(i, files[i - 1]);
				}
			}
			SubscribeSelectionChangedEvent();
		}

		public void UpdateListBoxItem(List<string> files)
		{
			if (SlideItems == null)
			{
				return;
			}
			for (int i = 0; i < SlideItems.Count; i++)
			{
				if (files[i] != null && !(SlideItems[i].IsPptThumbnail == "True"))
				{
					BitmapImage bitmapImage = new BitmapImage(new Uri(files[i]));
					SlideItems[i].BitmapImage = bitmapImage;
					SlideItems[i].IsPptThumbnail = "True";
				}
			}
		}

		private void CreateSlideItem(int index, string filePath)
		{
			bool flag = !string.IsNullOrEmpty(filePath);
			SlideItem item = new SlideItem
			{
				Index = index,
				IsPptThumbnail = flag.ToString(),
				BitmapImage = (flag ? new BitmapImage(new Uri(filePath)) : null)
			};
			SlideItems.Add(item);
		}

		private void UpdateTimerTick(object sender, EventArgs e)
		{
			if (IsExiting || !IsNeedUpdateTimer || !PptOperator.Instance.IsShowing)
			{
				return;
			}
			SyntacticSugar.TryCatchAction(() =>
			{
				if (IsNeedUpdateTimer)
				{
					UpdateButtonState();
					if (IsNeedUpdateTimer)
					{
						UpdateWindowOpacity();
						UpdateGestureAndMouseLineTrackState();
						WindowStateHelper.UpdateWindowRect(this);
						if (Opacity > 0.0)
						{
							WindowStateHelper.MakeAppAbovePpt(this);
						}
					}
				}
			});
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				PptOperator.Instance.Exit();
			}
			if (e.Key == Key.Prior || e.Key == Key.Left || e.Key == Key.Up)
			{
				PptOperator.Instance.PreviousPage();
			}
			if (e.Key == Key.Next || e.Key == Key.Right || e.Key == Key.Down)
			{
				PptOperator.Instance.NextPage();
			}
		}

		private void SlideListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				ListBox listBox = sender as ListBox;
				if (listBox != null)
				{
					int num = listBox.SelectedIndex + 1;
					SetPageIndex(num);
					PptOperator.Instance.GotoSlide(num);
					FridayReporter.ReportEventImmediately((listBox.Name == "LeftSlideListBox") ? EventID.LeftRegionThumbnailListBoxItem : EventID.RightRegionThumbnailListBoxItem);
				}
			});
		}

		private void ReleaseTimerTick(object sender, EventArgs e)
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				_releaseTimer.Stop();
				BitmapImage bitmapImage = new BitmapImage(new Uri(ReleaseMemoryImageHelper.GetImageFilePath()));
				if (SlideItems != null && SlideItems.Any())
				{
					foreach (SlideItem slideItem in SlideItems)
					{
						slideItem.BitmapImage = bitmapImage;
					}
				}
				CancelSubscribeSelectionChangedEvent();
				SlideItems = null;
				SubscribeSelectionChangedEvent();
				HiddenWindow();
				GarbageCollection.Collect();
			});
		}

		private void QrGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			QrImage.Visibility = Visibility.Collapsed;
			QrGrid.Visibility = Visibility.Collapsed;
		}

	}
}
