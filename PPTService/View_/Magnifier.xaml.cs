using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using PPTService.Collection;

namespace PPTService
{
	public partial class Magnifier : Window, IComponentConnector
	{
		private System.Windows.Point _mouseStartPoint;

		private System.Windows.Point _rectangleStartPoint;

		private double _magnifierBorderWidth;

		private double _magnifierBorderHeight;

		private bool _isMagnifierBorderMouseLeftButtonDown;

		private bool _isMagnifierRectangleMouseLeftButtonDown;

		private double _enlargementFactor = 2.0;

		private double _minEnlargementFactor = 1.0;

		private double _maxEnlargementFactor = 3.0;

		private static int _buttonBorderVerticalAlignmentMargin = 40;

		public Magnifier()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += Magnifier_Loaded;
			Unloaded += Magnifier_Unloaded;
			// 不要订阅 SizeChanged / LocationChanged 去调 SetWindowRect：
			// 放大镜是全屏固定窗口，尺寸只在 InitializeMagnifierWindow 里确定一次即可。
			// 之前订阅它们配合 SetWindowPos 改尺寸，形成重入循环、不断 PostMessage(WM_SIZE)
			// 耗尽 USER 对象配额，抛过 0x80004005「配额不足」。
			PreviewKeyDown += Magnifier_PreviewKeyDown;
			CloseButton.Click += (object s, RoutedEventArgs e) =>
			{
				Close();
				FridayReporter.ReportEventImmediately(EventID.MagnifierExit);
			};
		}

		private void Magnifier_Loaded(object sender, RoutedEventArgs e)
		{
			SetMagnifierShapeToRectangle();
			InitializeMagnifierWindow();
			PptOperator.Instance.ShowEnd += Instance_ShowEnd;
			// 布局稳定后：重新居中取景框 + 重算按钮位置 + 写诊断日志。
			// Loaded 时 RootGrid.ActualWidth/Height 常还是布局前默认值，导致按钮条被算到
			// 屏幕中间；拖动取景框后 LayoutUpdated 重算才归位。这里在后台优先级补一次定位。
			// 用无参 Action 的安全封装；之前用 DispatcherOperationCallback 又没传参数，
			// BeginInvoke 回调时抛 TargetParameterCountException「参数计数不匹配」导致程序崩溃。
			SyntacticSugar.CurrentDispatcherBeginInvoke(
				System.Windows.Threading.DispatcherPriority.Background,
				() =>
				{
					CenterRootGridAndButton();
					DumpMagnifierDiagnostics();
				});
		}

		/// <summary>把取景框居中到放映屏，并按其真实尺寸重算关灯/关闭按钮条位置。</summary>
		private void CenterRootGridAndButton()
		{
			Rectangle screenBounds = GetPresentationScreenBounds();
			Canvas.SetLeft(RootGrid, (screenBounds.Width - RootGrid.ActualWidth) / 2.0);
			Canvas.SetTop(RootGrid, (screenBounds.Height - RootGrid.ActualHeight) / 2.0);
			UpdateMagnifierImageViewBox();
			AdjustButtonPosition(Canvas.GetLeft(RootGrid), Canvas.GetTop(RootGrid));
		}

		/// <summary>
		/// 把放大镜窗口/截图位图/DPI/布局的真实数值写到 %APPDATA%\PPTServiceUltra\magnifier-diag.txt。
		/// 仅诊断用，不改变任何行为；异常一律吞掉，不影响放大镜正常工作。
		/// </summary>
		private void DumpMagnifierDiagnostics()
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("=== Magnifier diagnostic " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ===");
				Rectangle virtualScreen = SystemInformation.VirtualScreen;
				Rectangle presentationScreen = GetPresentationScreenBounds();
				sb.AppendLine("SystemInformation.VirtualScreen(px): " + virtualScreen.Left + "," + virtualScreen.Top + " " + virtualScreen.Width + "x" + virtualScreen.Height);
				sb.AppendLine("PresentationScreen(px): " + presentationScreen.Left + "," + presentationScreen.Top + " " + presentationScreen.Width + "x" + presentationScreen.Height);
				sb.AppendLine("SystemParameters.VirtualScreen(DIP): " + SystemParameters.VirtualScreenLeft + "," + SystemParameters.VirtualScreenTop + " " + SystemParameters.VirtualScreenWidth + "x" + SystemParameters.VirtualScreenHeight);
				sb.AppendLine("ScreenHelper.DpiX=" + ScreenHelper.DpiX + " DpiFactor=" + ScreenHelper.DpiFactor.ToString("F4"));
				PresentationSource source = PresentationSource.FromVisual(this);
				if (source != null && source.CompositionTarget != null)
				{
					sb.AppendLine("TransformToDevice(M11,M22)=" + source.CompositionTarget.TransformToDevice.M11.ToString("F4") + "," + source.CompositionTarget.TransformToDevice.M22.ToString("F4"));
				}
				else
				{
					sb.AppendLine("TransformToDevice: no PresentationSource");
				}
				BitmapSource bmp = RootCanvasBackgroundImage.ImageSource as BitmapSource;
				if (bmp != null)
				{
					sb.AppendLine("Snapshot bitmap: " + bmp.PixelWidth + "x" + bmp.PixelHeight + " px, DPI=" + bmp.DpiX.ToString("F2") + "x" + bmp.DpiY.ToString("F2"));
				}
				sb.AppendLine("Window Left/Top=" + Left + "," + Top + " Width/Height(DIP)=" + Width + "x" + Height);
				sb.AppendLine("Window ActualWidth/Height(DIP)=" + ActualWidth.ToString("F1") + "x" + ActualHeight.ToString("F1"));
				sb.AppendLine("RootCanvas ActualWidth/Height(DIP)=" + RootCanvas.ActualWidth.ToString("F1") + "x" + RootCanvas.ActualHeight.ToString("F1"));
				sb.AppendLine("RootCanvas Background Stretch=" + RootCanvasBackgroundImage.Stretch);
				sb.AppendLine("MagnifierImage.Viewbox=" + MagnifierImage.Viewbox);
				sb.AppendLine();
				string dir = AppPath.LocalRoot();
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				File.AppendAllText(System.IO.Path.Combine(dir, "magnifier-diag.txt"), sb.ToString(), Encoding.UTF8);
			}
			catch
			{
			}
		}

		private void Magnifier_Unloaded(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.ShowEnd -= Instance_ShowEnd;
		}

		private void Instance_ShowEnd(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(Close);
		}

		/// <summary>
		/// 取 PPT 放映窗口所在显示器的物理矩形。用户实测：同一台机器 Win10 4K 100% 无黑边、
		/// Win11 有黑边，且问题只在多屏虚拟桌面/多显示器 DPI 不一致时出现。
		/// 放映本来就是整屏，直接按放映窗口所在这一块屏幕截图并铺屏，
		/// 而不是用 SystemInformation.VirtualScreen 把所有显示器的虚拟桌面拼起来。
		/// </summary>
		private Rectangle GetPresentationScreenBounds()
		{
			// 用户设备：Windows 设置 3840×2160 @100%，4800×2700 根本设不了。
			// 但 SystemInformation.VirtualScreen / Screen.Bounds 在这台机器的 DPI 虚拟化下
			// 报 4800×2700（= 3840 × 1.25 的虚胖值）。用它做 Snapshot.Take 会 BitBlt 到
			// 真实屏幕之外，抓回全黑 → 位图右下 20% 黑边，这正是黑边的真正根因。
			// SystemParameters.VirtualScreen* 实测为 3840×2160，与用户在设置里看到的一致，
			// 截图和窗口都用它，就不会抓到屏幕外。
			return new Rectangle(
				(int)SystemParameters.VirtualScreenLeft,
				(int)SystemParameters.VirtualScreenTop,
				(int)SystemParameters.VirtualScreenWidth,
				(int)SystemParameters.VirtualScreenHeight);
		}

		private void InitializeMagnifierWindow()
		{
			Rectangle screenBounds = GetPresentationScreenBounds();
			RootCanvasBackgroundImage.ImageSource = Snapshot.Take(screenBounds);
			MagnifierImage.ImageSource = RootCanvasBackgroundImage.ImageSource;
			SetWindowRect();
			// Loaded 里设置截图源之前算过一次 Viewbox（那时只能回退到 DpiFactor）。
			// 截图与窗口尺寸就位后按真实像素/DIP 比值再算一次，保证首次打开取景正确。
			UpdateMagnifierImageViewBox();
		}

		/// <summary>
		/// 窗口 DIP 与物理像素的实际换算比例。用 WPF 运行时的 TransformToDevice，
		/// 而不是反射 SystemParameters.DpiX（后者在 4K 100% 缩放下曾得到 120，比实际高 25%，
		/// 导致窗口只有屏幕 80%、右下黑边）。拿不到 PresentationSource 时按 1:1 处理。
		/// </summary>
		private double GetDeviceScaleX()
		{
			PresentationSource source = PresentationSource.FromVisual(this);
			if (source != null && source.CompositionTarget != null)
			{
				double scale = source.CompositionTarget.TransformToDevice.M11;
				if (scale > 0.0)
				{
					return scale;
				}
			}
			return 1.0;
		}

		private double GetDeviceScaleY()
		{
			PresentationSource source = PresentationSource.FromVisual(this);
			if (source != null && source.CompositionTarget != null)
			{
				double scale = source.CompositionTarget.TransformToDevice.M22;
				if (scale > 0.0)
				{
					return scale;
				}
			}
			return 1.0;
		}

		private void SetWindowRect()
		{
			WindowStartupLocation = WindowStartupLocation.Manual;
			// 与 InitializeMagnifierWindow 的截图用同一个 SystemParameters 虚拟屏矩形：
			// SystemParameters.VirtualScreen* 是 WPF 的 DIP 值，实测=3840×2160，与用户在
			// Windows 设置里看到的 100% 3840×2160 一致。直接赋给 Left/Top/Width/Height 即可，
			// 不再除以 DpiFactor（那会把 3840 错误地缩成 3072）。
			// 位图与窗口用同一来源同一数值，Stretch=Fill 必然铺满，且不会 BitBlt 到屏幕外产生黑边。
			Rectangle screenBounds = GetPresentationScreenBounds();
			Left = screenBounds.Left;
			Top = screenBounds.Top;
			Width = (double)screenBounds.Width;
			Height = (double)screenBounds.Height;
			AdjustButtonBorderMargin();
			AdjustMagnifierBorder();
		}

		/// <summary>
		/// 截图像素 → 窗口 DIP 的换算比例。截图源与窗口都用 SystemParameters 虚拟屏 DIP，
		/// 但在 DPI 虚拟化设备上 Snapshot 位图像素仍可能 ≥ 窗口 DIP；用实际像素/实际 DIP 比值，
		/// 与窗口尺寸保持同一坐标系，避免反射 DpiFactor 不一致时取景框定位偏移。
		/// </summary>
		private double GetSnapshotScaleX()
		{
			BitmapSource source = RootCanvasBackgroundImage.ImageSource as BitmapSource;
			double dipWidth = ActualWidth > 0.0 ? ActualWidth : Width;
			if (source != null && source.PixelWidth > 0 && dipWidth > 0.0)
			{
				return (double)source.PixelWidth / dipWidth;
			}
			return ScreenHelper.DpiFactor;
		}

		private double GetSnapshotScaleY()
		{
			BitmapSource source = RootCanvasBackgroundImage.ImageSource as BitmapSource;
			double dipHeight = ActualHeight > 0.0 ? ActualHeight : Height;
			if (source != null && source.PixelHeight > 0 && dipHeight > 0.0)
			{
				return (double)source.PixelHeight / dipHeight;
			}
			return ScreenHelper.DpiFactor;
		}

		private void SetMagnifierShapeToRectangle()
		{
			MagnifierRectangle.RadiusX = 0.0;
			MagnifierRectangle.RadiusY = 0.0;
		}

		private void Magnifier_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				Close();
			}
		}

		private void ChangeBackgroundImageCheckBox_OnClick(object sender, RoutedEventArgs e)
		{
			FridayReporter.ReportEventImmediately(EventID.MagnifierChangeTransparency);
			if (ChangeBackgroundImageCheckBox.IsChecked == true)
			{
				(Resources["HiddenBackgroundImageStoryboard"] as Storyboard)?.Begin();
				FridayReporter.ReportEventImmediately(EventID.MagnifierNonTransparentState);
			}
			else
			{
				(Resources["ShowBackgroundImageStoryboard"] as Storyboard)?.Begin();
				FridayReporter.ReportEventImmediately(EventID.MagnifierTransparentState);
			}
		}

		/// <summary>
		/// 事件是否落在底部「关灯 / 关闭」按钮组上。ButtonBorder 是 RootCanvas 的子元素，
		/// 而 RootCanvas 挂了取景框的拖动 handler（无条件 e.Handled + CaptureDevice）。
		/// 触控点击按钮时事件会冒泡到 RootCanvas，若被取景框 handler 拦截并把触摸/触控笔
		/// 捕获到 MagnifierRectangle，CheckBox 的点击状态就无法刷新——表现为点了「开灯」
		/// 后按钮仍显示选中，直到点按钮外才恢复。因此按钮区的事件必须放行给按钮自己处理。
		/// </summary>
		private bool IsOnButtonArea(DependencyObject source)
		{
			DependencyObject dep = source;
			while (dep != null)
			{
				if (ReferenceEquals(dep, ButtonBorder))
				{
					return true;
				}
				dep = VisualTreeHelper.GetParent(dep);
			}
			return false;
		}

		private void MagnifierRectangle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// 按钮区（关灯 / 关闭）交给按钮自己处理，不要拦截或抢捕获。
			if (IsOnButtonArea(e.OriginalSource as DependencyObject))
			{
				return;
			}
			e.Handled = true;
			MagnifierRectangle.CaptureDevice();
			_mouseStartPoint = e.GetPosition(this);
			_rectangleStartPoint.X = Canvas.GetLeft(RootGrid);
			_rectangleStartPoint.Y = Canvas.GetTop(RootGrid);
			_isMagnifierRectangleMouseLeftButtonDown = true;
			LayoutUpdated += Magnifier_LayoutUpdated;
		}

		private void MagnifierRectangle_OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			// 只有真正在拖动取景框时才处理；经过按钮区不影响拖动。
			if (!_isMagnifierRectangleMouseLeftButtonDown)
			{
				return;
			}
			e.Handled = true;
			Vector vector = e.GetPosition(this) - _mouseStartPoint;
			double left = _rectangleStartPoint.X + vector.X;
			double top = _rectangleStartPoint.Y + vector.Y;
			AdjustMagnifierPosition(ref left, ref top);
			Canvas.SetLeft(RootGrid, left);
			Canvas.SetTop(RootGrid, top);
			UpdateMagnifierImageViewBox();
		}

		private void MagnifierRectangle_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// 未处于拖动状态时（例如只是点按钮），不要拦截，也不要释放捕获。
			if (!_isMagnifierRectangleMouseLeftButtonDown)
			{
				return;
			}
			e.Handled = true;
			_isMagnifierRectangleMouseLeftButtonDown = false;
			LayoutUpdated -= Magnifier_LayoutUpdated;
			MagnifierRectangle.ReleaseDeviceCapture();
			FridayReporter.ReportEventImmediately(EventID.MagnifierMove);
		}

		private void MagnifierBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			(sender as Border)?.CaptureDevice();
			_magnifierBorderWidth = MagnifierBorder.ActualWidth;
			_magnifierBorderHeight = MagnifierBorder.ActualHeight;
			_rectangleStartPoint.X = Canvas.GetLeft(RootGrid);
			_rectangleStartPoint.Y = Canvas.GetTop(RootGrid);
			_mouseStartPoint = e.GetPosition(this);
			_isMagnifierBorderMouseLeftButtonDown = true;
			LayoutUpdated += Magnifier_LayoutUpdated;
		}

		private void MagnifierBorder_OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			e.Handled = true;
			if (!_isMagnifierBorderMouseLeftButtonDown)
			{
				return;
			}
			Border border = e.OriginalSource as Border;
			if (border == null)
			{
				return;
			}
			Vector vector = e.GetPosition(this) - _mouseStartPoint;
			switch (border.Name)
			{
			case "LeftTopThumbBorder":
				if (_magnifierBorderWidth - vector.X * 2.0 > MagnifierBorder.MinWidth && _magnifierBorderWidth - vector.X * 2.0 < MagnifierBorder.MaxWidth)
				{
					MagnifierBorder.Width = _magnifierBorderWidth - vector.X * 2.0;
					Canvas.SetLeft(RootGrid, _rectangleStartPoint.X + vector.X);
				}
				if (_magnifierBorderHeight - vector.Y * 2.0 > MagnifierBorder.MinHeight && _magnifierBorderHeight - vector.Y * 2.0 < MagnifierBorder.MaxHeight)
				{
					MagnifierBorder.Height = _magnifierBorderHeight - vector.Y * 2.0;
					Canvas.SetTop(RootGrid, _rectangleStartPoint.Y + vector.Y);
				}
				break;
			case "RightTopThumbBorder":
				if (_magnifierBorderWidth + vector.X * 2.0 > MagnifierBorder.MinWidth && _magnifierBorderWidth + vector.X * 2.0 < MagnifierBorder.MaxWidth)
				{
					MagnifierBorder.Width = _magnifierBorderWidth + vector.X * 2.0;
					Canvas.SetLeft(RootGrid, _rectangleStartPoint.X - vector.X);
				}
				if (_magnifierBorderHeight - vector.Y * 2.0 > MagnifierBorder.MinHeight && _magnifierBorderHeight - vector.Y * 2.0 < MagnifierBorder.MaxHeight)
				{
					MagnifierBorder.Height = _magnifierBorderHeight - vector.Y * 2.0;
					Canvas.SetTop(RootGrid, _rectangleStartPoint.Y + vector.Y);
				}
				break;
			case "LeftBottomThumbBorder":
				if (_magnifierBorderWidth - vector.X * 2.0 > MagnifierBorder.MinWidth && _magnifierBorderWidth - vector.X * 2.0 < MagnifierBorder.MaxWidth)
				{
					MagnifierBorder.Width = _magnifierBorderWidth - vector.X * 2.0;
					Canvas.SetLeft(RootGrid, _rectangleStartPoint.X + vector.X);
				}
				if (_magnifierBorderHeight + vector.Y * 2.0 > MagnifierBorder.MinHeight && _magnifierBorderHeight + vector.Y * 2.0 < MagnifierBorder.MaxHeight)
				{
					MagnifierBorder.Height = _magnifierBorderHeight + vector.Y * 2.0;
					Canvas.SetTop(RootGrid, _rectangleStartPoint.Y - vector.Y);
				}
				break;
			case "RightBottomThumbBorder":
				if (_magnifierBorderWidth + vector.X * 2.0 > MagnifierBorder.MinWidth && _magnifierBorderWidth + vector.X * 2.0 < MagnifierBorder.MaxWidth)
				{
					MagnifierBorder.Width = _magnifierBorderWidth + vector.X * 2.0;
					Canvas.SetLeft(RootGrid, _rectangleStartPoint.X - vector.X);
				}
				if (_magnifierBorderHeight + vector.Y * 2.0 > MagnifierBorder.MinHeight && _magnifierBorderHeight + vector.Y * 2.0 < MagnifierBorder.MaxHeight)
				{
					MagnifierBorder.Height = _magnifierBorderHeight + vector.Y * 2.0;
					Canvas.SetTop(RootGrid, _rectangleStartPoint.Y - vector.Y);
				}
				break;
			}
			AdjustButtonPosition(Canvas.GetLeft(RootGrid), Canvas.GetTop(RootGrid));
		}

		private void MagnifierBorder_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			_isMagnifierBorderMouseLeftButtonDown = false;
			LayoutUpdated -= Magnifier_LayoutUpdated;
			(sender as Border)?.ReleaseDeviceCapture();
			FridayReporter.ReportEventImmediately(EventID.MagnifierScaleFrame);
		}

		private void MagnifierRectangle_OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
		{
			e.ManipulationContainer = RootCanvas;
			e.Mode = ManipulationModes.Translate | ManipulationModes.Scale;
			LayoutUpdated += Magnifier_LayoutUpdated;
		}

		private void MagnifierRectangle_OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			Enlarge(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
		}

		private void MagnifierRectangle_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			LayoutUpdated -= Magnifier_LayoutUpdated;
			FridayReporter.ReportEventImmediately(EventID.MagnifierScaleContent);
		}

		private void MagnifierRectangle_OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			double scaleX = 1.05;
			if (e.Delta < 0)
			{
				scaleX = 0.95;
			}
			Enlarge(scaleX, 0.0, 0.0);
		}

		private void Enlarge(double scaleX, double translationX, double translationY)
		{
			double num = _enlargementFactor * scaleX;
			if (!(num < _minEnlargementFactor) && !(num > _maxEnlargementFactor))
			{
				_enlargementFactor = num;
				UpdateMagnifierImageViewBox();
				if (!(Math.Abs(scaleX - 1.0) > 1E-07))
				{
					double left = Canvas.GetLeft(RootGrid) + translationX;
					double top = Canvas.GetTop(RootGrid) + translationY;
					AdjustMagnifierPosition(ref left, ref top);
					Canvas.SetLeft(RootGrid, left);
					Canvas.SetTop(RootGrid, top);
				}
			}
		}

		private void Magnifier_LayoutUpdated(object sender, EventArgs e)
		{
			UpdateMagnifierImageViewBox();
		}

		private void UpdateMagnifierImageViewBox()
		{
			double num = Canvas.GetLeft(RootGrid) + InnerMagnifierBorder.BorderThickness.Left;
			double num2 = Canvas.GetTop(RootGrid) + InnerMagnifierBorder.BorderThickness.Top;
			double num3 = MagnifierRectangle.ActualWidth - MagnifierRectangle.ActualWidth / _enlargementFactor;
			double num4 = MagnifierRectangle.ActualHeight - MagnifierRectangle.ActualHeight / _enlargementFactor;
			num += num3 / 2.0;
			num2 += num4 / 2.0;
			double num5 = num + MagnifierRectangle.ActualWidth / _enlargementFactor;
			double num6 = num2 + MagnifierRectangle.ActualHeight / _enlargementFactor;
			// 取景框坐标是窗口 DIP，Viewbox 用的是截图像素；用实际像素/DIP 比值换算，
			// 与窗口尺寸（SystemParameters DIP）保持同一坐标系，避免反射 DpiFactor 不一致。
			double scaleX = GetSnapshotScaleX();
			double scaleY = GetSnapshotScaleY();
			num *= scaleX;
			num2 *= scaleY;
			num5 *= scaleX;
			num6 *= scaleY;
			MagnifierImage.Viewbox = new Rect(new System.Windows.Point(num, num2), new System.Windows.Point(num5, num6));
		}

		private void AdjustMagnifierPosition(ref double left, ref double top)
		{
			double left2 = MagnifierBorder.Margin.Left;
			if (left < (0.0 - MagnifierBorder.ActualWidth) / 2.0 - left2)
			{
				left = (0.0 - MagnifierBorder.ActualWidth) / 2.0 - left2;
			}
			if (top < (0.0 - MagnifierBorder.ActualHeight) / 2.0 - left2)
			{
				top = (0.0 - MagnifierBorder.ActualHeight) / 2.0 - left2;
			}
			if (left > ActualWidth - MagnifierBorder.ActualWidth / 2.0 - left2)
			{
				left = ActualWidth - MagnifierBorder.ActualWidth / 2.0 - left2;
			}
			if (top > ActualHeight - MagnifierBorder.ActualHeight / 2.0 - left2)
			{
				top = ActualHeight - MagnifierBorder.ActualHeight / 2.0 - left2;
			}
			AdjustButtonPosition(left, top);
		}

		/// <summary>按钮条与放大镜可见矩形下边缘的填充间距（原来约 30px，改为 15px）。</summary>
		private const double ButtonRectGap = 15.0;

		/// <summary>
		/// 工具条贴在放大镜可见矩形下边缘外面、水平居中，两个按钮平分。
		/// 可见放大镜矩形就是 RootGrid（InnerMagnifierBorder / MagnifierRectangle 撑满它）；
		/// MagnifierBorder 带 Margin=-30，是缩放手柄层、比可见矩形大一圈，不能作定位基准
		/// ——否则它的 -30 会把间距抵消掉，改间距看不出效果。
		/// left/top 为 RootGrid 在 RootCanvas 中的位置，尺寸取 RootGrid 实际宽高。
		/// 工具条尺寸沿用原版固定值（不随矩形宽度拉伸），仅按分辨率分档。
		/// </summary>
		private void AdjustButtonPosition(double left, double top)
		{
			double rectWidth = RootGrid.ActualWidth > 0.0 ? RootGrid.ActualWidth : RootGrid.Width;
			double rectHeight = RootGrid.ActualHeight > 0.0 ? RootGrid.ActualHeight : RootGrid.Height;
			double buttonWidth = ButtonBorder.Width;
			// 水平居中于可见矩形；垂直贴在矩形下边缘外面（间距可控）。
			Canvas.SetLeft(ButtonBorder, left + (rectWidth - buttonWidth) / 2.0);
			Canvas.SetTop(ButtonBorder, top + rectHeight + ButtonRectGap);
		}

		private void AdjustButtonBorderMargin()
		{
			// 工具条尺寸按分辨率分档（沿用原版 100x52 / 80x44 / 200x104）。
			if (Height > 2100.0)
			{
				ButtonBorder.Width = 200;
				ButtonBorder.Height = 104;
			}
			if (Height > 770.0 && Height <= 2100.0)
			{
				ButtonBorder.Width = 100;
				ButtonBorder.Height = 52;
			}
			if (Height > 0.0 && Height <= 770.0)
			{
				ButtonBorder.Width = 80;
				ButtonBorder.Height = 44;
			}
			AdjustButtonPosition(Canvas.GetLeft(RootGrid), Canvas.GetTop(RootGrid));
		}

		private void AdjustMagnifierBorder()
		{
			double num = 450.0;
			if (Height > 2100.0)
			{
				num = 900.0;
			}
			if (Height > 770.0 && Height <= 2100.0)
			{
				num = 450.0;
			}
			if (Height > 0.0 && Height <= 770.0)
			{
				num = 300.0;
			}
			MagnifierBorder.Width = num;
			MagnifierBorder.Height = num;
			MagnifierBorder.MaxWidth = Width;
			MagnifierBorder.MaxHeight = Height;
		}

		public void Operate(System.Windows.Point point, double length)
		{
			double left = point.X;
			double top = point.Y;
			AdjustMagnifierPosition(ref left, ref top);
			RootGrid.Width = length;
			RootGrid.Height = length;
			Canvas.SetLeft(RootGrid, left);
			Canvas.SetTop(RootGrid, top);
			UpdateMagnifierImageViewBox();
		}

	}
}
