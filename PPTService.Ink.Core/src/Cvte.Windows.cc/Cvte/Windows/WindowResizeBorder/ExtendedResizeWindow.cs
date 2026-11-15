using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using PPTService.Ink.Core.Windows.Native;

namespace PPTService.Ink.Core.Windows.WindowResizeBorder
{
	internal class ExtendedResizeWindow : Window
	{
		private HitTestValues _hitTestValue;

		private Cursor _cursor;

		private double _top;

		private double _bottom;

		public Direction Direction { get; set; }

		public Border GlowBorder { get; }

		public ExtendedResizeWindow(Direction dir)
		{
			Direction = dir;
			SetStyle(dir);
			Title = "Felix Shadow Window";
			Background = Brushes.Transparent;
			ShowInTaskbar = false;
			ShowActivated = false;
			ResizeMode = ResizeMode.NoResize;
			WindowStyle = WindowStyle.None;
			WindowChrome.SetWindowChrome(this, new WindowChrome
			{
				GlassFrameThickness = new Thickness(-1.0),
				CaptionHeight = 0.0,
				ResizeBorderThickness = new Thickness(0.0)
			});
			SnapsToDevicePixels = true;
			GlowBorder = new Border
			{
				SnapsToDevicePixels = true,
				IsHitTestVisible = false,
				Background = Brushes.Black
			};
			if (!Win32.Dwmapi.DwmIsCompositionEnabled())
			{
				AllowsTransparency = true;
			}
			Content = GlowBorder;
		}

		public void Render(Rect bounds, ExtendedResizeBorder extendedResizeBorder)
		{
			Opacity = 1.0 / 255.0;
			Thickness thickness = Normalize(new Thickness(1.0));
			Thickness radius = extendedResizeBorder.Radius;
			Rect rect = new Rect(new Point(bounds.Left + thickness.Left, bounds.Top + thickness.Top), new Point(bounds.Right - thickness.Right, bounds.Bottom - thickness.Bottom));
			Rect rect2 = new Rect(new Point(bounds.Left - radius.Left, bounds.Top - radius.Top), new Point(bounds.Right + radius.Right, bounds.Bottom + radius.Bottom));
			rect = Normalize(rect);
			rect2 = Normalize(rect2);
			_top = radius.Top;
			_bottom = _top + bounds.Height;
			switch (Direction)
			{
			case Direction.Left:
				UpdateBounds(new Rect(new Point(rect2.Left, rect2.Top), new Point(rect.Left, rect2.Bottom)));
				GlowBorder.HorizontalAlignment = HorizontalAlignment.Left;
				GlowBorder.Width = radius.Left + thickness.Left;
				break;
			case Direction.Right:
				UpdateBounds(new Rect(new Point(rect.Right, rect2.Top), new Point(rect2.Right, rect2.Bottom)));
				GlowBorder.HorizontalAlignment = HorizontalAlignment.Right;
				GlowBorder.Width = radius.Right + radius.Right;
				break;
			case Direction.Top:
				UpdateBounds(new Rect(new Point(rect.Left, rect2.Top), rect.TopRight));
				GlowBorder.VerticalAlignment = VerticalAlignment.Top;
				GlowBorder.Height = radius.Top + radius.Top;
				break;
			case Direction.Bottom:
				UpdateBounds(new Rect(rect.BottomLeft, new Point(rect.Right, rect2.Bottom)));
				GlowBorder.VerticalAlignment = VerticalAlignment.Bottom;
				GlowBorder.Height = radius.Bottom + radius.Bottom;
				break;
			}
		}

		private void SetStyle(Direction direction)
		{
			ValueTuple<HitTestValues, Cursor> valueTuple = BuildStyle(direction);
			HitTestValues item = valueTuple.Item1;
			Cursor item2 = valueTuple.Item2;
			_hitTestValue = item;
			_cursor = item2;
		}

		private ValueTuple<HitTestValues, Cursor> BuildStyle(Direction direction)
		{
			HitTestValues item = HitTestValues.HTLEFT;
			Cursor item2 = Cursors.AppStarting;
			switch (direction)
			{
			case Direction.Left:
				item = HitTestValues.HTLEFT;
				item2 = Cursors.SizeWE;
				break;
			case Direction.Top:
				item = HitTestValues.HTTOP;
				item2 = Cursors.SizeNS;
				break;
			case Direction.Right:
				item = HitTestValues.HTRIGHT;
				item2 = Cursors.SizeWE;
				break;
			case Direction.Bottom:
				item = HitTestValues.HTBOTTOM;
				item2 = Cursors.SizeNS;
				break;
			case Direction.TopLeft:
				item = HitTestValues.HTTOPLEFT;
				item2 = Cursors.SizeNWSE;
				break;
			case Direction.TopRight:
				item = HitTestValues.HTTOPRIGHT;
				item2 = Cursors.SizeNESW;
				break;
			case Direction.BottomLeft:
				item = HitTestValues.HTBOTTOMLEFT;
				item2 = Cursors.SizeNESW;
				break;
			case Direction.BottomRight:
				item = HitTestValues.HTBOTTOMRIGHT;
				item2 = Cursors.SizeNWSE;
				break;
			}
			return new ValueTuple<HitTestValues, Cursor>(item, item2);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			IntPtr handle = new WindowInteropHelper(this).Handle;
			HwndSource.FromHwnd(handle)?.AddHook(WndProc);
			int windowLong = Win32.GetWindowLong(handle, -20);
			Win32.ExtendedWindowStyles extendedWindowStyles = Win32.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
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
			case WM.MOUSEFIRST:
				if (Direction == Direction.Right || Direction == Direction.Left)
				{
					int i = lParam.ToInt32() >> 16;
					UpdateCourse(i);
				}
				break;
			}
			return IntPtr.Zero;
		}

		private void UpdateCourse(int i)
		{
			if ((double)i < _top)
			{
				if (Direction == Direction.Right)
				{
					SetStyle(Direction.TopRight);
				}
				else if (Direction == Direction.Left)
				{
					SetStyle(Direction.TopLeft);
				}
			}
			else if ((double)i > _bottom)
			{
				if (Direction == Direction.Right)
				{
					SetStyle(Direction.BottomRight);
				}
				else if (Direction == Direction.Left)
				{
					SetStyle(Direction.BottomLeft);
				}
			}
			else if (Direction == Direction.Right)
			{
				SetStyle(Direction.Right);
			}
			else if (Direction == Direction.Left)
			{
				SetStyle(Direction.Left);
			}
		}

		private void UpdateBounds(Rect bounds)
		{
			Left = bounds.Left;
			Top = bounds.Top;
			Width = bounds.Width;
			Height = bounds.Height;
		}

		private Rect Normalize(Rect rect)
		{
			return new Rect(new Point(Math.Round(rect.Left), Math.Round(rect.Top)), new Point(Math.Round(rect.Right), Math.Round(rect.Bottom)));
		}

		private Thickness Normalize(Thickness t)
		{
			return new Thickness(Math.Round(t.Left), Math.Round(t.Top), Math.Round(t.Right), Math.Round(t.Bottom));
		}
	}
}
