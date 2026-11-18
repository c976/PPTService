using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
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
		}

		private void SetWindowRect()
		{
			Rectangle virtualScreen = SystemInformation.VirtualScreen;
			WindowStartupLocation = WindowStartupLocation.Manual;
			Left = virtualScreen.Left;
			Top = virtualScreen.Top;
			Width = (double)virtualScreen.Width / ScreenHelper.DpiFactor;
			Height = (double)virtualScreen.Height / ScreenHelper.DpiFactor;
			AdjustButtonBorderMargin();
			AdjustMagnifierBorder();
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
			num *= ScreenHelper.DpiFactor;
			num2 *= ScreenHelper.DpiFactor;
			num5 *= ScreenHelper.DpiFactor;
			num6 *= ScreenHelper.DpiFactor;
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

		/// <summary>
		/// 按钮组贴在放大镜矩形下边缘外面、水平居中。left/top 为 RootGrid 在 RootCanvas
		/// 中的位置；可见放大镜矩形是 MagnifierBorder（位于 RootGrid 内、Margin=-30），
		/// 其在 RootCanvas 中的左上角 = left + Margin.Left、尺寸 = MagnifierBorder.Width/Height。
		/// 用 Canvas.Left/Top 定位，按钮随放大镜一起移动 / 缩放。
		/// </summary>
		private void AdjustButtonPosition(double left, double top)
		{
			double margin = MagnifierBorder.Margin.Left;
			double rectWidth = MagnifierBorder.Width;
			double rectHeight = MagnifierBorder.Height;
			double buttonWidth = ButtonBorder.Width;
			// 水平居中于矩形；垂直贴在矩形下边缘外面（留一点点间距）。
			Canvas.SetLeft(ButtonBorder, left + margin + (rectWidth - buttonWidth) / 2.0);
			Canvas.SetTop(ButtonBorder, top + margin + rectHeight + 2.0);
		}

		private void AdjustButtonBorderMargin()
		{
			// 按钮尺寸按分辨率分档（与 XAML 默认 100x52 对应）。
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
