using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
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
			SizeChanged += (object s, SizeChangedEventArgs e) =>
			{
				SetWindowRect();
			};
			LocationChanged += (object s, EventArgs e) =>
			{
				SetWindowRect();
			};
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
			Rect screenBoundsByWindow = ScreenHelper.GetScreenBoundsByWindow(System.Windows.Application.Current.MainWindow);
			Canvas.SetLeft(RootGrid, screenBoundsByWindow.Left - Left + (screenBoundsByWindow.Width - RootGrid.ActualWidth) / 2.0);
			Canvas.SetTop(RootGrid, screenBoundsByWindow.Top - Top + (screenBoundsByWindow.Height - RootGrid.ActualHeight) / 2.0);
			UpdateMagnifierImageViewBox();
			InitializeMagnifierWindow();
			PptOperator.Instance.ShowEnd += Instance_ShowEnd;
		}

		private void Magnifier_Unloaded(object sender, RoutedEventArgs e)
		{
			PptOperator.Instance.ShowEnd -= Instance_ShowEnd;
		}

		private void Instance_ShowEnd(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherBeginInvoke(Close);
		}

		private void InitializeMagnifierWindow()
		{
			Rectangle virtualScreen = SystemInformation.VirtualScreen;
			RootCanvasBackgroundImage.ImageSource = Snapshot.Take(virtualScreen);
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
			// 必须用「屏幕真实尺寸 ÷ 窗口运行时 DPI 缩放」来设窗口，两者取自同一坐标系：
			//   - 屏幕尺寸用 Snapshot 的位图像素（SystemInformation.VirtualScreen 抓取，物理像素），
			//     这是背景图 Inflation 铺满所依据的真实像素数；
			//   - 缩放用 TransformToDevice（本窗口 DIP→物理的真实比例）。
			// 之前用 SystemParameters.VirtualScreenWidth 得到的 3072 是被反射 DPI 缩过的逻辑值，
			// 而窗口按 96 DIP 渲染，于是只覆盖 3840 屏幕的 80%，右下留黑边。
			BitmapSource snapshot = RootCanvasBackgroundImage.ImageSource as BitmapSource;
			Rectangle virtualScreen = SystemInformation.VirtualScreen;
			double physicalWidth = snapshot != null && snapshot.PixelWidth > 0 ? snapshot.PixelWidth : virtualScreen.Width;
			double physicalHeight = snapshot != null && snapshot.PixelHeight > 0 ? snapshot.PixelHeight : virtualScreen.Height;
			double scaleX = GetDeviceScaleX();
			double scaleY = GetDeviceScaleY();
			Left = virtualScreen.Left / scaleX;
			Top = virtualScreen.Top / scaleY;
			Width = physicalWidth / scaleX;
			Height = physicalHeight / scaleY;
			AdjustButtonBorderMargin();
			AdjustMagnifierBorder();
		}

		/// <summary>
		/// 截图像素 → 窗口 DIP 的换算比例。截图由 SystemInformation.VirtualScreen（物理像素）抓取，
		/// 窗口用 SystemParameters（DIP）定位，两者比值即真实缩放。用实际像素/实际 DIP 计算而不是
		/// 反射的 ScreenHelper.DpiFactor，避免 DPI 感知不一致时取景框定位偏移。
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

		private void MagnifierRectangle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
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
			e.Handled = true;
			if (_isMagnifierRectangleMouseLeftButtonDown)
			{
				Vector vector = e.GetPosition(this) - _mouseStartPoint;
				double left = _rectangleStartPoint.X + vector.X;
				double top = _rectangleStartPoint.Y + vector.Y;
				AdjustMagnifierPosition(ref left, ref top);
				Canvas.SetLeft(RootGrid, left);
				Canvas.SetTop(RootGrid, top);
				UpdateMagnifierImageViewBox();
			}
		}

		private void MagnifierRectangle_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
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
