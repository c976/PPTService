using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows
{
	internal class Shadow : Window
	{
		private HitTestValues _hitTestValue;

		private Cursor _cursor;

		private bool _canResize;

		public Direction Direction { get; set; }

		public Border GlowBorder { get; }

		public Border WindowOuterBorder { get; }

		public Shadow(Direction dir, bool canResize = true)
		{
			Direction = dir;
			SetStyle();
			_canResize = canResize;
			Title = "Felix Shadow Window";
			Background = Brushes.Transparent;
			ShowInTaskbar = false;
			ShowActivated = false;
			ResizeMode = ResizeMode.NoResize;
			AllowsTransparency = true;
			WindowStyle = WindowStyle.None;
			SnapsToDevicePixels = true;
			GlowBorder = new Border
			{
				SnapsToDevicePixels = true,
				IsHitTestVisible = false
			};
			if (!Win32.Dwmapi.DwmIsCompositionEnabled())
			{
				AllowsTransparency = true;
			}
			WindowOuterBorder = new Border();
			GlowBorder.Child = WindowOuterBorder;
			Content = GlowBorder;
		}

		public void Render(Rect bounds, ShadowBorder shadowBorder)
		{
			Opacity = shadowBorder.Opacity;
			Thickness radius = shadowBorder.Radius;
			Thickness thickness = Normalize(shadowBorder.Shrink);
			Color color = shadowBorder.Color;
			Thickness thickness2 = new Thickness(radius.Left - thickness.Left, radius.Top - thickness.Top, radius.Right - thickness.Right, radius.Bottom - thickness.Bottom);
			Rect rect = new Rect(new Point(bounds.Left + thickness.Left, bounds.Top + thickness.Top), new Point(bounds.Right - thickness.Right, bounds.Bottom - thickness.Bottom));
			Rect rect2 = new Rect(new Point(bounds.Left - thickness2.Left, bounds.Top - thickness2.Top), new Point(bounds.Right + thickness2.Right, bounds.Bottom + thickness2.Bottom));
			switch (Direction)
			{
			case Direction.Left:
				UpdateBounds(new Rect(new Point(rect2.Left, rect.Top), rect.BottomLeft));
				GlowBorder.Background = LinearGradientBrush(color, radius.Left / thickness2.Left, new Point(0.0, 0.0), new Point(1.0, 0.0), shadowBorder.GradientFunc);
				GlowBorder.HorizontalAlignment = HorizontalAlignment.Left;
				GlowBorder.Width = thickness2.Left;
				WindowOuterBorder.Width = shadowBorder.WindowOutterBorderThickness.Left;
				WindowOuterBorder.HorizontalAlignment = HorizontalAlignment.Right;
				WindowOuterBorder.Background = new SolidColorBrush(shadowBorder.WindowOutterBorderColor);
				break;
			case Direction.Right:
				UpdateBounds(new Rect(rect.TopRight, new Point(rect2.Right, rect.Bottom)));
				GlowBorder.Background = LinearGradientBrush(color, radius.Right / thickness2.Right, new Point(1.0, 0.0), new Point(0.0, 0.0), shadowBorder.GradientFunc);
				GlowBorder.HorizontalAlignment = HorizontalAlignment.Right;
				GlowBorder.Width = thickness2.Right;
				WindowOuterBorder.Width = shadowBorder.WindowOutterBorderThickness.Right;
				WindowOuterBorder.HorizontalAlignment = HorizontalAlignment.Left;
				WindowOuterBorder.Background = new SolidColorBrush(shadowBorder.WindowOutterBorderColor);
				break;
			case Direction.Top:
				UpdateBounds(new Rect(new Point(rect.Left, rect2.Top), rect.TopRight));
				GlowBorder.Background = LinearGradientBrush(color, radius.Top / thickness2.Top, new Point(0.0, 0.0), new Point(0.0, 1.0), shadowBorder.GradientFunc);
				GlowBorder.VerticalAlignment = VerticalAlignment.Top;
				GlowBorder.Height = thickness2.Top;
				WindowOuterBorder.Height = shadowBorder.WindowOutterBorderThickness.Top;
				WindowOuterBorder.VerticalAlignment = VerticalAlignment.Bottom;
				WindowOuterBorder.Background = new SolidColorBrush(shadowBorder.WindowOutterBorderColor);
				break;
			case Direction.Bottom:
				UpdateBounds(new Rect(rect.BottomLeft, new Point(rect.Right, rect2.Bottom)));
				GlowBorder.Background = LinearGradientBrush(color, radius.Bottom / thickness2.Bottom, new Point(0.0, 1.0), new Point(0.0, 0.0), shadowBorder.GradientFunc);
				GlowBorder.VerticalAlignment = VerticalAlignment.Bottom;
				GlowBorder.Height = thickness2.Bottom;
				WindowOuterBorder.Height = shadowBorder.WindowOutterBorderThickness.Bottom;
				WindowOuterBorder.VerticalAlignment = VerticalAlignment.Top;
				WindowOuterBorder.Background = new SolidColorBrush(shadowBorder.WindowOutterBorderColor);
				break;
			case Direction.TopLeft:
				UpdateBounds(new Rect(rect.TopLeft, rect2.TopLeft));
				GlowBorder.Background = RadialGradientBrush(color, new Point(1.0, 1.0), shadowBorder.GradientFunc);
				WindowOuterBorder.Width = thickness.Left + shadowBorder.WindowOutterBorderThickness.Left;
				WindowOuterBorder.Height = thickness.Top + shadowBorder.WindowOutterBorderThickness.Top;
				WindowOuterBorder.HorizontalAlignment = HorizontalAlignment.Right;
				WindowOuterBorder.VerticalAlignment = VerticalAlignment.Bottom;
				WindowOuterBorder.Background = new SolidColorBrush(shadowBorder.WindowOutterBorderColor);
				OpacityMask = new DrawingBrush(new GeometryDrawing(Brushes.Black, null, new CombinedGeometry
				{
					Geometry1 = new RectangleGeometry(new Rect(new Size(radius.Left, radius.Top))),
					Geometry2 = new RectangleGeometry(new Rect(thickness2.Left, thickness2.Top, thickness.Left, thickness.Top)),
					GeometryCombineMode = GeometryCombineMode.Exclude
				}));
				break;
			case Direction.TopRight:
				UpdateBounds(new Rect(rect.TopRight, rect2.TopRight));
				GlowBorder.Background = RadialGradientBrush(color, new Point(0.0, 1.0), shadowBorder.GradientFunc);
				WindowOuterBorder.Width = thickness.Right + shadowBorder.WindowOutterBorderThickness.Right;
				WindowOuterBorder.Height = thickness.Top + shadowBorder.WindowOutterBorderThickness.Top;
				WindowOuterBorder.HorizontalAlignment = HorizontalAlignment.Left;
				WindowOuterBorder.VerticalAlignment = VerticalAlignment.Bottom;
				WindowOuterBorder.Background = new SolidColorBrush(shadowBorder.WindowOutterBorderColor);
				OpacityMask = new DrawingBrush(new GeometryDrawing(Brushes.Black, null, new CombinedGeometry
				{
					Geometry1 = new RectangleGeometry(new Rect(new Size(radius.Right, radius.Top))),
					Geometry2 = new RectangleGeometry(new Rect(0.0, thickness2.Top, thickness.Right, thickness.Top)),
					GeometryCombineMode = GeometryCombineMode.Exclude
				}));
				break;
			case Direction.BottomLeft:
				UpdateBounds(new Rect(rect.BottomLeft, rect2.BottomLeft));
				GlowBorder.Background = RadialGradientBrush(color, new Point(1.0, 0.0), shadowBorder.GradientFunc);
				WindowOuterBorder.Width = thickness.Left + shadowBorder.WindowOutterBorderThickness.Left;
				WindowOuterBorder.Height = thickness.Bottom + shadowBorder.WindowOutterBorderThickness.Bottom;
				WindowOuterBorder.HorizontalAlignment = HorizontalAlignment.Right;
				WindowOuterBorder.VerticalAlignment = VerticalAlignment.Top;
				WindowOuterBorder.Background = new SolidColorBrush(shadowBorder.WindowOutterBorderColor);
				OpacityMask = new DrawingBrush(new GeometryDrawing(Brushes.Black, null, new CombinedGeometry
				{
					Geometry1 = new RectangleGeometry(new Rect(new Size(radius.Left, radius.Bottom))),
					Geometry2 = new RectangleGeometry(new Rect(thickness2.Left, 0.0, thickness.Left, thickness.Bottom)),
					GeometryCombineMode = GeometryCombineMode.Exclude
				}));
				break;
			case Direction.BottomRight:
				UpdateBounds(new Rect(rect.BottomRight, rect2.BottomRight));
				GlowBorder.Background = RadialGradientBrush(color, new Point(0.0, 0.0), shadowBorder.GradientFunc);
				WindowOuterBorder.Width = thickness.Right + shadowBorder.WindowOutterBorderThickness.Right;
				WindowOuterBorder.Height = thickness.Bottom + shadowBorder.WindowOutterBorderThickness.Bottom;
				WindowOuterBorder.HorizontalAlignment = HorizontalAlignment.Left;
				WindowOuterBorder.VerticalAlignment = VerticalAlignment.Top;
				WindowOuterBorder.Background = new SolidColorBrush(shadowBorder.WindowOutterBorderColor);
				OpacityMask = new DrawingBrush(new GeometryDrawing(Brushes.Black, null, new CombinedGeometry
				{
					Geometry1 = new RectangleGeometry(new Rect(new Size(radius.Right, radius.Bottom))),
					Geometry2 = new RectangleGeometry(new Rect(0.0, 0.0, thickness.Right, thickness.Bottom)),
					GeometryCombineMode = GeometryCombineMode.Exclude
				}));
				break;
			}
		}

		private void SetStyle()
		{
			switch (Direction)
			{
			case Direction.Left:
				_hitTestValue = HitTestValues.HTLEFT;
				_cursor = Cursors.SizeWE;
				break;
			case Direction.Top:
				_hitTestValue = HitTestValues.HTTOP;
				_cursor = Cursors.SizeNS;
				break;
			case Direction.Right:
				_hitTestValue = HitTestValues.HTRIGHT;
				_cursor = Cursors.SizeWE;
				break;
			case Direction.Bottom:
				_hitTestValue = HitTestValues.HTBOTTOM;
				_cursor = Cursors.SizeNS;
				break;
			case Direction.TopLeft:
				_hitTestValue = HitTestValues.HTTOPLEFT;
				_cursor = Cursors.SizeNWSE;
				break;
			case Direction.TopRight:
				_hitTestValue = HitTestValues.HTTOPRIGHT;
				_cursor = Cursors.SizeNESW;
				break;
			case Direction.BottomLeft:
				_hitTestValue = HitTestValues.HTBOTTOMLEFT;
				_cursor = Cursors.SizeNESW;
				break;
			case Direction.BottomRight:
				_hitTestValue = HitTestValues.HTBOTTOMRIGHT;
				_cursor = Cursors.SizeNWSE;
				break;
			}
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			IntPtr handle = new WindowInteropHelper(this).Handle;
			HwndSource.FromHwnd(handle)?.AddHook(WndProc);
			int windowLong = Win32.GetWindowLong(handle, -20);
			Win32.ExtendedWindowStyles extendedWindowStyles = Win32.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
			if (!_canResize)
			{
				extendedWindowStyles |= Win32.ExtendedWindowStyles.WS_EX_TRANSPARENT;
			}
			Win32.SetWindowLong(handle, -20, windowLong | (int)extendedWindowStyles);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch ((WM)msg)
			{
			case WM.MOUSEACTIVATE:
				handled = true;
				return new IntPtr(3);
			case WM.LBUTTONDOWN:
				if (IsHitTestVisible)
				{
					Win32.PostMessage(new WindowInteropHelper(Owner).Handle, 161, (int)_hitTestValue, 0);
				}
				break;
			case WM.GETMINMAXINFO:
			{
				Win32.MINMAXINFO mINMAXINFO = (Win32.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(Win32.MINMAXINFO));
				if (mINMAXINFO.ptMaxSize.x > 0)
				{
					mINMAXINFO.ptMaxSize.x = (mINMAXINFO.ptMaxSize.y = (mINMAXINFO.ptMaxTrackSize.x = (mINMAXINFO.ptMaxTrackSize.y = int.MaxValue)));
					Marshal.StructureToPtr((object)mINMAXINFO, lParam, true);
				}
				break;
			}
			case WM.NCHITTEST:
			{
				Cursor cursor = _cursor;
				if (cursor != null && cursor != Cursor)
				{
					Cursor = cursor;
				}
				break;
			}
			}
			return IntPtr.Zero;
		}

		private void Interpolate(GradientBrush brush, Func<double, double> func)
		{
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				double num2 = 1.0 / (double)num * (double)(i + 1);
				double ratio = func?.Invoke(num2) ?? Math.Pow(num2, 2.0);
				Color color = Interpolate(brush.GradientStops.First().Color, brush.GradientStops.Last().Color, ratio);
				double offset = Interpolate(brush.GradientStops.First().Offset, brush.GradientStops.Last().Offset, num2);
				GradientStop value = new GradientStop
				{
					Offset = offset,
					Color = color
				};
				brush.GradientStops.Insert(brush.GradientStops.Count - 1, value);
			}
		}

		private Color Interpolate(Color color1, Color color2, double ratio)
		{
			return Color.FromArgb((byte)((double)(int)color1.A + (double)(color2.A - color1.A) * ratio), (byte)((double)(int)color1.R + (double)(color2.R - color1.R) * ratio), (byte)((double)(int)color1.G + (double)(color2.G - color1.G) * ratio), (byte)((double)(int)color1.B + (double)(color2.B - color1.B) * ratio));
		}

		private double Interpolate(double d1, double d2, double ratio)
		{
			return d1 + (d2 - d1) * ratio;
		}

		private LinearGradientBrush LinearGradientBrush(Color color, double total, Point startPoint, Point endPoint, Func<double, double> func)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new GradientStopCollection
			{
				new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 0.0),
				new GradientStop(color, total)
			})
			{
				StartPoint = startPoint,
				EndPoint = endPoint
			};
			Interpolate(linearGradientBrush, func);
			return linearGradientBrush;
		}

		private RadialGradientBrush RadialGradientBrush(Color color, Point center, Func<double, double> func)
		{
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush(new GradientStopCollection
			{
				new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 1.0),
				new GradientStop(color, 0.0)
			})
			{
				Center = center,
				GradientOrigin = center,
				RadiusX = 1.0,
				RadiusY = 1.0
			};
			Interpolate(radialGradientBrush, func);
			return radialGradientBrush;
		}

		private void UpdateBounds(Rect bounds)
		{
			Left = bounds.Left;
			Top = bounds.Top;
			Width = bounds.Width;
			Height = bounds.Height;
		}

		private Thickness Normalize(Thickness t)
		{
			return new Thickness(Math.Round(t.Left), Math.Round(t.Top), Math.Round(t.Right), Math.Round(t.Bottom));
		}
	}
}
