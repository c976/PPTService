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
using PPTService.Core.Windows;
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
			// 翻页按钮位置交由用户配置决定（默认沿用程序原逻辑），运行时切换也复用此方法。
			ApplyPageTurningLayout();
			BottomCenterButtonGrid.Children.Add(ViewHelper.BottomArea);
			AnnotationController.Attach(this);
			AnnotationController.SetEnabled(UserConfigManager.Load().UseSelfAnnotation);
			Width = SystemParameters.PrimaryScreenWidth;
			Height = SystemParameters.PrimaryScreenHeight;
			SubscribeSelectionChangedEvent();
			_gesture = new Gesture();
			_gesture.ChangeGestureScope();
			WindowsApi.SetPopupParameter();
			PptOperator.Instance.ShowBegin += _pptOperatorShowBegin;
			PptOperator.Instance.ShowEnd += _pptOperatorShowEnd;
			PptOperator.Instance.NextSlide += PptOperatorNextSlide;
			// 应用用户配置：工具栏按钮显隐/顺序、翻页按钮位置
			UserConfigManager.ApplyCurrent();
		}

		/// <summary>
		/// 把自实现批注的绘制层（Board 控件）挂到视觉树最底层；传入 null 即移除。
		/// 工具栏按钮网格位于其之上，WPF 单窗口命中测试保证按钮永远可点。
		/// </summary>
		public void SetAnnotationLayer(UIElement layer)
		{
			AnnotationHost.Child = layer;
		}

		/// <summary>
		/// 移除自实现批注绘制层。
		/// </summary>
		public void RemoveAnnotationLayer()
		{
			AnnotationHost.Child = null;
		}

		/// <summary>
		/// 按用户配置决定翻页按钮放在「两边」还是「底部」，并把缩略图导航面板对齐到对应位置。
		/// 仅将当前生效的那一对翻页控件加入可视树，避免网格透明度动画同时影响两套控件。
		/// </summary>
		public void ApplyPageTurningLayout()
		{
			bool useBottom = UserConfigManager.GetPageTurningLayout() == "Bottom";

			if (LeftCenterButtonGrid.Children.Contains(ViewHelper.LeftArea))
			{
				LeftCenterButtonGrid.Children.Remove(ViewHelper.LeftArea);
			}
			if (RightCenterButtonGrid.Children.Contains(ViewHelper.RightArea))
			{
				RightCenterButtonGrid.Children.Remove(ViewHelper.RightArea);
			}
			if (LeftBottomButtonGrid.Children.Contains(ViewHelper.LeftBottomArea))
			{
				LeftBottomButtonGrid.Children.Remove(ViewHelper.LeftBottomArea);
			}
			if (RightBottomButtonGrid.Children.Contains(ViewHelper.RightBottomArea))
			{
				RightBottomButtonGrid.Children.Remove(ViewHelper.RightBottomArea);
			}

			if (useBottom)
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
					AnnotationController.SetEnabled(UserConfigManager.Load().UseSelfAnnotation);
					AnnotationController.ShowPage(PptOperator.Instance.CurrentShowPosition);
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
				// 自实现批注跟随 PPT 翻页切换对应页笔迹
				AnnotationController.ShowPage(PptOperator.Instance.CurrentShowPosition);
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
			ViewHelper.BottomArea.IsInnerBoardWindowShowing = ViewHelper.InnerBoardWindow.IsShowing;
			if (ViewHelper.BottomArea.IsSelfAnnotationActive)
			{
				// 自实现批注下按钮状态由工具栏自己维护；Office 指针类型已不再代表当前模式，
				// 若继续用它刷新，300ms 定时器会把笔/橡皮立刻重置回「鼠标」。
				return;
			}
			PptOperator.Instance.UpdatePointerColor();
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
			// 模拟放映没有真实缩略图，跳过列表选中逻辑。
			if (DebugMode.IsEnabled)
			{
				return;
			}
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
			if (DebugMode.IsEnabled)
			{
				// 没有真实 PPT 文件可供导出缩略图，跳过分析直接进入放映态。
				_isAnalyzing = false;
				DoAfterAnalyzePpt();
				return;
			}
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
						// 放映结束：隐藏批注窗口（下次放映开始时按配置重新显示）。
						AnnotationController.Hide();
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
				// 模拟放映时保持主窗口键盘焦点（ESC / 方向键翻页）。
				if (!DebugMode.IsEnabled)
				{
					WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
				}
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
			// 自实现批注下也必须维持工具栏的显隐与「浮在 PPT 放映窗口之上」：
			// 否则工具栏会被 PPT 盖住而「按钮整体消失」。批注层已合并进本窗口，
			// 不再需要跨窗口 SetWindowPos；且 UpdateButtonState() 在自实现批注时已跳过
			// Office COM（不会触发之前的「画第一笔卡死」），故定时器照常运行即可。
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
