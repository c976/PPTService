using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.Win32;

namespace PPTService
{
	public class ScreenHelper : INotifyPropertyChanged
	{
		private struct POINT
		{
			public int X;

			public int Y;

			public static implicit operator System.Windows.Point(POINT point)
			{
				return new System.Windows.Point(point.X, point.Y);
			}
		}

		private static Rect _oldMainWindowScreenBounds;

		public static int DpiX { get; private set; }

		public static int DpiY { get; private set; }

		public static double DpiFactor { get; private set; }

		public static Rect MainWindowBounds
		{
			get
			{
				if (System.Windows.Application.Current.MainWindow == null)
				{
					return new Rectangle(new System.Drawing.Point(0, 0), SystemInformation.PrimaryMonitorSize).ToRect(DpiX);
				}
				return GetWindowBounds(System.Windows.Application.Current.MainWindow);
			}
		}

		public static Rect MainWindowActualBounds
		{
			get
			{
				if (System.Windows.Application.Current.MainWindow == null)
				{
					return new Rectangle(new System.Drawing.Point(0, 0), SystemInformation.PrimaryMonitorSize).ToRect(DpiX);
				}
				return GetWindowActualBounds(System.Windows.Application.Current.MainWindow);
			}
		}

		public static Rect MainWindowScreenBounds
		{
			get
			{
				Rect screenBoundsFromPoint = GetScreenBoundsFromPoint(MainWindowActualBounds.Location + new Vector(MainWindowActualBounds.Width / 2.0, MainWindowActualBounds.Height / 2.0));
				if (_oldMainWindowScreenBounds == default(Rect))
				{
					_oldMainWindowScreenBounds = screenBoundsFromPoint;
				}
				else if (_oldMainWindowScreenBounds.Size != screenBoundsFromPoint.Size)
				{
					_oldMainWindowScreenBounds = screenBoundsFromPoint;
				}
				return screenBoundsFromPoint;
			}
		}

		public static int ScreenCount => Screen.AllScreens.Length;

		public static int MainWindowScreenIndex => ScreenRects.FindIndex((Rect rect) => rect.Contains(MainWindowBounds.X + MainWindowBounds.Width / 2.0, MainWindowBounds.Y + MainWindowBounds.Height / 2.0));

		public static List<Rect> ScreenRects => Screen.AllScreens.Select((Screen screen) => screen.Bounds.ToRect()).ToList();

		public static event EventHandler DisplaySettingsChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		public static Rect GetScreenBoundsByWindow(Window window)
		{
			return GetScreenBoundsFromPoint(GetWindowBounds(window).Location + new Vector(window.Width / 2.0, window.Height / 2.0));
		}

		static ScreenHelper()
		{
			SystemEvents.DisplaySettingsChanged += (object sender, EventArgs args) =>
			{
				Update();
				OnDisplaySettingsChanged();
			};
			Update();
		}

		private static void Update()
		{
			PropertyInfo property = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic);
			PropertyInfo property2 = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic);
			DpiX = ((property != null) ? ((int)property.GetValue(null, null)) : 96);
			DpiY = ((property2 != null) ? ((int)property2.GetValue(null, null)) : 96);
			DpiFactor = (double)DpiX / 96.0;
		}

		private static void OnDisplaySettingsChanged()
		{
			DisplaySettingsChanged?.Invoke(null, EventArgs.Empty);
		}

		public static double GetRelativePixel(double pixel)
		{
			Rect mainWindowScreenBounds = MainWindowScreenBounds;
			if (mainWindowScreenBounds.Width > mainWindowScreenBounds.Height)
			{
				return pixel * MainWindowScreenBounds.Height / 1080.0;
			}
			return pixel * MainWindowScreenBounds.Width / 1080.0;
		}

		public static Rect GetScreenBoundsFromPoint(System.Windows.Point point)
		{
			return Screen.GetBounds(point.ToWinFormPoint()).ToRect(DpiX);
		}

		public static Rect GetWorkingArea(System.Windows.Point point)
		{
			return Screen.GetWorkingArea(point.ToWinFormPoint()).ToRect();
		}

		public static Rect GetWindowBounds(Window window)
		{
			return new Rect(window.Left, window.Top, window.Width, window.Height);
		}

		public static Rect GetWindowActualBounds(Window window)
		{
			System.Windows.Point point = window.PointToScreen(default(System.Windows.Point)).ToWpfPoint();
			return new Rect(point.X, point.Y, window.ActualWidth, window.ActualHeight);
		}

		public static System.Windows.Point GetMousePosition()
		{
			POINT lpPoint;
			GetCursorPos(out lpPoint);
			return lpPoint;
		}

		public static System.Windows.Point GetMousePosition(Visual relativeToVisual)
		{
			POINT lpPoint;
			GetCursorPos(out lpPoint);
			return relativeToVisual.PointFromScreen(new System.Windows.Point(lpPoint.X, lpPoint.Y));
		}

		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out POINT lpPoint);

		protected virtual void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
