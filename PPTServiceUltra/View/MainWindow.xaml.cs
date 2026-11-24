using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PPTServiceUltra.Core.Windows;
using Microsoft.Office.Interop.PowerPoint;
using PPTServiceUltra.Collection;

namespace PPTServiceUltra
{
	public partial class MainWindow : Window, IComponentConnector
	{
		private Gesture _gesture;

		private bool _isAnalyzing;

		private int _currentPageIndex;

		private string _prePptFilePath;

		/// <summary>是否允许窗口真正关闭；仅系统注销/关机或程序内部主动退出时置 true。</summary>
		private bool _allowClose;

		private static readonly int TimerSpan = 300;

		private static readonly int ReleaseTimerSpan = 6000;

		private readonly DispatcherTimer _releaseTimer;

		// 工具栏收起 / 拉起状态：只影响中心工具栏，翻页按钮始终保留。
		private bool _toolbarCollapsed;
		private bool _toolbarAnimating;

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
			// 主窗口是常驻工具栏，禁止用户用 Alt+F4 / 系统菜单 / 任务栏等常规途径关掉它，
			// 并从 Alt+Tab 中隐藏（见 EnableCloseProtection）。
			EnableCloseProtection();
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
			PreviewMouseDown += MainWindow_PreviewMouseDown;
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

		/// <summary>
		/// 启用主窗口关闭保护：拦截 Closing / WM_SYSCOMMAND(SC_CLOSE) / WM_CLOSE，
		/// 阻止 Alt+F4、标题栏关闭、任务栏右键关闭、任务管理器「结束任务」等常规关闭途径；
		/// 并给窗口加 WS_EX_TOOLWINDOW，从 Alt+Tab 与任务栏中隐藏。
		/// 系统注销 / 关机（WM_QUERYENDSESSION / WM_ENDSESSION）与程序内部
		/// 显式调用 AllowClose() 仍可正常退出，避免卡住系统。
		/// </summary>
		public void EnableCloseProtection()
		{
			Closing += MainWindow_Closing;
			SourceInitialized += MainWindow_SourceInitialized;
			System.Windows.Application.Current.SessionEnding += delegate { _allowClose = true; };
		}

		/// <summary>程序内部需要真正退出时（如调试模式结束放映）放行一次关闭。</summary>
		public void AllowClose()
		{
			_allowClose = true;
		}

		private void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			if (!_allowClose)
			{
				e.Cancel = true;
			}
		}

		private void MainWindow_SourceInitialized(object sender, EventArgs e)
		{
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			if (hwnd == IntPtr.Zero)
			{
				return;
			}
			// 保留原有的 WS_EX_LAYERED（透明窗口必需），只追加 TOOLWINDOW、清掉 APPWINDOW。
			uint exStyle = WindowsApi.GetWindowLong(hwnd, WindowsApi.WindowFlags.GwlExstyle);
			exStyle |= WindowsApi.WindowFlags.WsExToolWindow;
			exStyle &= ~WindowsApi.WindowFlags.WsExAppWindow;
			// 默认靠 Owner 关系保持在 PPT 放映窗口之上，不进全局置顶组；
			// 「批注窗口始终置顶（兼容旧版）」开关打开时，Loaded 阶段会重新置上该位。
			exStyle &= ~WindowsApi.WindowFlags.WsExTopmost;
			WindowsApi.SetWindowLong(hwnd, WindowsApi.WindowFlags.GwlExstyle, exStyle);
			// SWP_FRAMECHANGED 让样式立即生效（NOMOVE/NOSIZE/NOZORDER/NOACTIVATE 保持位置与层级）。
			WindowsApi.SetWindowPos(hwnd, 0, 0, 0, 0, 0, 0x0037);
			HwndSource source = HwndSource.FromHwnd(hwnd);
			if (source != null)
			{
				source.AddHook(MainWindow_WndProc);
			}
		}

		private IntPtr MainWindow_WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == WindowsApi.WindowFlags.WmQueryEndSession || msg == WindowsApi.WindowFlags.WmEndSession)
			{
				_allowClose = true;
			}
			else if (!_allowClose)
			{
				if (msg == WindowsApi.WindowFlags.WmClose ||
					(msg == WindowsApi.WindowFlags.WmSysCommand &&
					 (wParam.ToInt32() & 0xFFF0) == WindowsApi.WindowFlags.ScClose))
				{
					handled = true;
				}
			}
			return IntPtr.Zero;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// Office 原生批注下点击 PPT 放映窗口不会进入本窗口的路由事件，
			// 用全局鼠标按下兜底收起工具栏弹窗（处理器内部只在该模式生效）。
			if (MouseHelper.Instance != null)
			{
				MouseHelper.Instance.Down += GlobalMouseDownForOfficeAnnotation;
			}
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
			string layout = UserConfigManager.GetPageTurningLayout();

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

			// 不显示翻页按钮：仅移除四块翻页区域控件即可，缩略图导航面板保持原逻辑。
			if (layout == "None")
			{
				return;
			}

			bool useBottom = layout == "Bottom";
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
					// ResetToolbarCollapsedState 会写 BottomCenterButtonGrid / PullupButton 等 UI 元素，
					// 必须在 UI 线程执行。放映开始事件来自 COM 后台线程，直接调用会抛
					// InvalidOperationException（调用线程无法访问此对象），异常被 TryCatchAction
					// 吞掉后 Show()/Opacity=1 都不会执行 —— 表现为重新放映后工具栏不显示。
					// 每场放映都按当前演示程序重排翻页按钮：Office/WPS 切换时
					// PptVersionManager 会变（Office2016+ 底部，其余两侧），但工具栏窗口
					// 是同一个、不会重建，此前只在构造/改设置时排一次，导致换软件后按钮
					// 停留在上次的位置（或要等 3~5s 某处触发才跳过去）。
					ApplyPageTurningLayout();
					ResetToolbarCollapsedState();
					_releaseTimer?.Stop();
					Show();
					// 显示态先行：XAML 初始 Opacity="0"，必须尽早置 1。
					// 下面 SetPageIndex / ShowRangeType 等会访问 COM，放映刚启动时
					// 可能被 RPC_E_CALL_REJECTED 拒绝而抛异常；整个回调只在更外层的
					// TryCatchAction 里兜底，若 Opacity 排在后面，异常会让它停在 0
					// ——表现为「放 PPT 后工具栏不显示，右键一下（COM 通了）才出现」。
					IsExiting = false;
					Opacity = 1.0;
					// 不再 Activate() 抢前台：把焦点留给 PPT 放映窗口，翻页笔 / 方向键
					// 由 PowerPoint 原生处理（debug 由模拟放映窗口处理），也不会把用户
					// 刚打开的第三方窗口焦点顶掉。z 序改由 Owner 关系保证「只在 PPT 之上」。
					// 注：笔 / 橡皮模式下 SetEnabled → ApplyFocusForMode 会把焦点夺回工具栏，
					// 翻页键改由 MainWindow_KeyDown 转发 COM（见 AnnotationController）。
					WindowStateHelper.MakeAppAbovePpt(this);
					WindowsApi.SetForegroundWindow(PptOperator.Instance.SlideShowWindowIntPtr);
					AdjustFontSizeAndStyle();
					ViewHelper.BottomArea?.InitializeBottomArea();
					// 批注层状态同步不依赖任何 COM 调用，必须先做且不受后面 COM 失败影响：
					// 之前这些都排在 SetPageIndex/ShowPage 等 COM 调用之后，放映刚启动时
					// 一个 RPC_E_CALL_REJECTED 就会跳过它们，导致工具栏显示了但批注层没启用、
					// 按钮选中态没同步（笔与鼠标同时选中 / 批注关不掉）。
					AnnotationController.SetEnabled(UserConfigManager.Load().UseSelfAnnotation);
					// 重新放映 / 切换 PPT：清掉上一场残留笔迹，再切到当前页。
					// 下面的 COM 调用各自独立保护，任何一个失败都只跳过它自己，
					// 不再中断整条工具栏/批注状态初始化链。
					AnnotationController.Reset();
					SyntacticSugar.TryComAction(() =>
					{
						_currentPageIndex = PptOperator.Instance.CurrentShowPosition;
					});
					SyntacticSugar.TryComAction(() => SetPageIndex(_currentPageIndex));
					SyntacticSugar.TryComAction(() => SetLeftRightOpacity());
					SyntacticSugar.TryComAction(() =>
						AnnotationController.ShowPage(PptOperator.Instance.CurrentShowPosition));
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

		/// <summary>设置里切换「自动淡化」后立即生效：关闭时立刻恢复不透明，开启时重启淡化计时。</summary>
		public void ApplyPageButtonFadeState(bool autoFade)
		{
			if (autoFade)
			{
				PlayHiddenLeftCenterButtonGridStoryboard();
				PlayHiddenRightCenterButtonGridStoryboard();
				return;
			}
			KeepPageButtonsOpaque(LeftCenterButtonGrid, LeftBottomButtonGrid,
				RightCenterButtonGrid, RightBottomButtonGrid);
		}

		/// <summary>放映中一段时间未点翻页按钮后淡化；受「自动淡化」设置控制。</summary>
		private void KeepPageButtonsOpaque(params FrameworkElement[] targets)
		{
			foreach (FrameworkElement target in targets)
			{
				if (target == null)
				{
					continue;
				}
				// 移除正在运行的淡化动画，避免 7 秒后仍把按钮降到 0.3。
				target.BeginAnimation(UIElement.OpacityProperty, null);
				target.Opacity = 1.0;
			}
		}

		public void PlayHiddenLeftCenterButtonGridStoryboard()
		{
			if (!UserConfigManager.Load().AutoFadePageButtons)
			{
				KeepPageButtonsOpaque(LeftCenterButtonGrid, LeftBottomButtonGrid);
				return;
			}
			(Resources["HiddenLeftCenterButtonGridStoryBoard"] as Storyboard)?.Begin();
		}

		public void PlayHiddenRightCenterButtonGridStoryboard()
		{
			if (!UserConfigManager.Load().AutoFadePageButtons)
			{
				KeepPageButtonsOpaque(RightCenterButtonGrid, RightBottomButtonGrid);
				return;
			}
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

		/// <summary>收起中心工具栏（结束放映按钮右侧的按钮触发），翻页按钮不动。</summary>
		public void CollapseToolbar()
		{
			if (_toolbarCollapsed || _toolbarAnimating)
			{
				return;
			}
			_toolbarCollapsed = true;
			_toolbarAnimating = true;
			BottomCenterButtonGrid.IsHitTestVisible = false;
			PullupButton.IsHitTestVisible = false;
			(Resources["HideToolbarStoryboard"] as Storyboard)?.Begin();
		}

		/// <summary>点底部中央的拉起按钮恢复工具栏。</summary>
		public void ExpandToolbar()
		{
			if (!_toolbarCollapsed || _toolbarAnimating)
			{
				return;
			}
			_toolbarCollapsed = false;
			_toolbarAnimating = true;
			PullupButton.IsHitTestVisible = false;
			(Resources["HideToolbarPullupStoryboard"] as Storyboard)?.Begin();
		}

		/// <summary>新一场放映 / 放映结束：无条件回到展开状态，避免上一场残留。</summary>
		public void ResetToolbarCollapsedState()
		{
			string[] names = { "HideToolbarStoryboard", "ShowToolbarStoryboard", "ShowToolbarPullupStoryboard", "HideToolbarPullupStoryboard" };
			foreach (string name in names)
			{
				(Resources[name] as Storyboard)?.Stop(this);
			}
			_toolbarCollapsed = false;
			_toolbarAnimating = false;
			BottomCenterButtonGrid.Visibility = Visibility.Visible;
			BottomCenterButtonGrid.Opacity = 1.0;
			BottomCenterButtonGrid.IsHitTestVisible = true;
			((TranslateTransform)BottomCenterButtonGrid.RenderTransform).Y = 0.0;
			PullupButton.Visibility = Visibility.Collapsed;
			PullupButton.Opacity = 0.0;
			PullupButton.IsHitTestVisible = false;
		}

		private void PullupToolbar_OnClick(object sender, RoutedEventArgs e)
		{
			ExpandToolbar();
		}

		private void HideToolbarStoryboard_OnCompleted(object sender, EventArgs e)
		{
			_toolbarAnimating = false;
			BottomCenterButtonGrid.Visibility = Visibility.Collapsed;
			if (!_toolbarCollapsed)
			{
				return;
			}
			PullupButton.Visibility = Visibility.Visible;
			PullupButton.IsHitTestVisible = true;
			(Resources["ShowToolbarPullupStoryboard"] as Storyboard)?.Begin();
		}

		private void ShowToolbarStoryboard_OnCompleted(object sender, EventArgs e)
		{
			_toolbarAnimating = false;
			BottomCenterButtonGrid.Opacity = 1.0;
			BottomCenterButtonGrid.IsHitTestVisible = true;
			((TranslateTransform)BottomCenterButtonGrid.RenderTransform).Y = 0.0;
			PullupButton.Visibility = Visibility.Collapsed;
			PullupButton.IsHitTestVisible = false;
		}

		private void HideToolbarPullupStoryboard_OnCompleted(object sender, EventArgs e)
		{
			PullupButton.Visibility = Visibility.Collapsed;
			if (_toolbarCollapsed || !_toolbarAnimating)
			{
				return;
			}
			BottomCenterButtonGrid.Visibility = Visibility.Visible;
			BottomCenterButtonGrid.Opacity = 0.0;
			BottomCenterButtonGrid.IsHitTestVisible = true;
			(Resources["ShowToolbarStoryboard"] as Storyboard)?.Begin();
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
			// PPT 忙时 COM 会被拒绝（RPC_E_CALL_REJECTED）。退避期间跳过本轮探测，
			// 既不刷日志也不让 300ms 定时器反复阻塞 UI 线程；按钮状态由用户操作直接更新。
			if (!SyntacticSugar.IsComBusy)
			{
				PptOperator.Instance.UpdatePointerColor();
				if (!ViewHelper.InnerBoardWindow.IsShowing)
				{
					ViewHelper.BottomArea.UpdateButtonState(PptOperator.Instance.PointerType);
				}
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
			// 设置窗口打开期间：全屏半透明遮罩下用户仍可能左右滑动，必须持续停掉手势，
			// 否则横滑被识别成翻页（无论自身 Gesture 还是 PowerPoint 原生手势）。
			if (ViewHelper.SettingsWindowOpen || IsTransparent || ViewHelper.MainWindow.IsLeftSlideManagerViewShowed || ViewHelper.MainWindow.IsRightSlideManagerViewShowed || ViewHelper.BottomArea.IsNeedStopMouseLineTrack)
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
					else if (UserConfigManager.Load().ShowLoadingSpinner)
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
								Restart.PPTServiceUltra();
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
					else
					{
						// 不显示加载转圈：跳过 LoadingView，直接完成分析后的初始化与回调。
						InitListBoxItem();
						DoAfterAnalyzePpt();
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
					// 放映结束也回到展开态，下一场放映直接显示完整工具栏。
					ResetToolbarCollapsedState();
					// 放映窗口即将销毁：先解除 Owner，避免系统连带销毁本窗口（下次放映还要复用）。
					WindowStateHelper.DetachFromPpt(this);
					if (ConfigHelper.Instance.IsNeedStart() || !IsTransparent)
					{
						Opacity = 0.0;
						UpdateTimer.Stop();
						MouseHelper.Instance.Stop();
						ViewHelper.BottomArea.UpdateButtonState(PpSlideShowPointerType.ppSlideShowPointerNone);
						ViewHelper.InnerBoardWindow.HiddenAndClearInkCanvas();
						// 放映结束：只隐藏批注层（下次放映开始时按配置重新显示）。
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
					// 放映结束：笔色 / 粗细 / 模式 / 弹窗 / 滑杆全部复位，含板中板自己的 Board。
					// 顺序：先复位注解控制器真值源，再让工具栏与板中板按新真值同步；
					// 最后统一切回「鼠标」——HiddenAndClearInkCanvas() 会经板中板
					// OnInnerBoardShowingChanged(false) 恢复 _modeBeforeInnerBoard（可能是笔），
					// 若不覆盖，下次放映 SetEnabled() 会把 Board 切回笔，出现
					// 「工具栏显示鼠标、实际是笔」的不一致。
					AnnotationController.ResetPenSettings();
					ViewHelper.BottomArea?.ResetPenSettings();
					ViewHelper.ResetInnerBoardPenSettingsIfCreated();
					AnnotationController.ResetModeToMouse();
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

		/// <summary>
		/// 弹窗（笔颜色盘 / 清屏滑杆）打开时点击其外部：只收起弹窗、不处理事件，
		/// 让这次鼠标按下继续路由到批注层 Board，实现「点窗口外的第一击即落笔 / 擦除」。
		/// 工具栏按钮交给各自 Click 处理开合，避免抢先关闭导致「再点一次又打开」。
		/// </summary>
		/// <summary>
		/// Office 原生批注模式的兜底：点击落在 PPT 放映窗口时本窗口收不到
		/// PreviewMouseDown，颜色盘无法靠路由事件收起。全局鼠标轮询不依赖窗口
		/// 消息路由，这里据此收起弹窗；仍排除弹窗与工具栏区域以免误触。
		/// 自实现批注由 PreviewMouseDown 的「落在 Board」判定处理，本方法不介入。
		/// </summary>
		private void GlobalMouseDownForOfficeAnnotation(object sender, MouseHelperEventArgs e)
		{
			if (UserConfigManager.Load().UseSelfAnnotation)
			{
				return;
			}
			BottomArea area = ViewHelper.BottomArea;
			if (area == null || !area.HasOpenToolPopup)
			{
				return;
			}
			double factor = ScreenHelper.DpiFactor;
			if (factor <= 0.0)
			{
				factor = 1.0;
			}
			Point screen = new Point(e.MousePoint.X * factor, e.MousePoint.Y * factor);
			if (area.IsScreenPointInToolArea(screen))
			{
				return;
			}
			area.CloseToolPopups();
		}

		private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			// 只有明确落在批注绘制层的按下才收起弹窗：这一击会继续路由到 Board，
			// 直接落第一笔 / 擦除。弹窗内空白、工具栏空白、主窗口空白、翻页区域
			// 一律不关，避免误触把刚打开的颜色盘 / 清屏滑杆关掉。
			if (!AnnotationController.IsPointerOnAnnotationBoard(e.OriginalSource as DependencyObject))
			{
				// 批注 Board 在笔 / 橡皮模式下会捕获鼠标与触控。若一次书写结束后捕获
				// 未被释放，后续点击工具栏 / 翻页按钮会被 Board 吞掉，表现为「画过一笔
				// 之后翻页按钮、翻页笔都不响应」。点击非绘制区时主动清掉残留捕获。
				if (Mouse.Captured != null)
				{
					Mouse.Capture(null);
				}
				if (Stylus.Captured != null)
				{
					Stylus.Capture(null);
				}
				return;
			}
			ViewHelper.BottomArea?.CloseToolPopups();
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
