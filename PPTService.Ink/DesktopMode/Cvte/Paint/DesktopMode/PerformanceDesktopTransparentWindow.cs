using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using Cvte.Windows.Controls;
using Cvte.Windows.Media;
using Cvte.Windows.Native;

namespace Cvte.Paint.DesktopMode
{
	public class PerformanceDesktopTransparentWindow : SafeWindow
	{
		private struct STYLESTRUCT
		{
			public int styleOld;

			public int styleNew;
		}

		private readonly bool _dwmEnabled;

		private readonly IntPtr _hwnd;

		public PerformanceDesktopTransparentWindow()
		{
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			WindowChrome.SetWindowChrome(this, new WindowChrome
			{
				GlassFrameThickness = WindowChrome.GlassFrameCompleteThickness,
				CaptionHeight = 0.0
			});
			FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(Border));
			frameworkElementFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
			FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ContentPresenter));
			frameworkElementFactory2.SetValue(UIElement.ClipToBoundsProperty, true);
			frameworkElementFactory.AppendChild(frameworkElementFactory2);
			Template = new ControlTemplate
			{
				TargetType = typeof(Window),
				VisualTree = frameworkElementFactory
			};
			_dwmEnabled = Win32.Dwmapi.DwmIsCompositionEnabled();
			if (_dwmEnabled)
			{
				_hwnd = new WindowInteropHelper(this).EnsureHandle();
				Loaded += PerformanceDesktopTransparentWindow_Loaded;
				Background = Brushes.Transparent;
			}
			else
			{
				AllowsTransparency = true;
				Background = BrushCreator.GetOrCreate("#01000000");
				_hwnd = new WindowInteropHelper(this).EnsureHandle();
			}
		}

		private void PerformanceDesktopTransparentWindow_Loaded(object sender, RoutedEventArgs e)
		{
			((HwndSource)PresentationSource.FromVisual(this)).AddHook(delegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
			{
				if (msg == 124 && (long)wParam == -20)
				{
					STYLESTRUCT sTYLESTRUCT = (STYLESTRUCT)Marshal.PtrToStructure(lParam, typeof(STYLESTRUCT));
					sTYLESTRUCT.styleNew |= 524288;
					Marshal.StructureToPtr((object)sTYLESTRUCT, lParam, false);
					handled = true;
				}
				return IntPtr.Zero;
			});
		}

		public void SetTransparentHitThrough()
		{
			if (_dwmEnabled)
			{
				Win32.User32.SetWindowLongPtr(_hwnd, Win32.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)(int)((long)Win32.User32.GetWindowLongPtr(_hwnd, Win32.GetWindowLongFields.GWL_EXSTYLE) | 0x20));
			}
			else
			{
				Background = Brushes.Transparent;
			}
		}

		public void SetTransparentNotHitThrough()
		{
			if (_dwmEnabled)
			{
				Win32.User32.SetWindowLongPtr(_hwnd, Win32.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)(int)((long)Win32.User32.GetWindowLongPtr(_hwnd, Win32.GetWindowLongFields.GWL_EXSTYLE) & -33));
			}
			else
			{
				Background = BrushCreator.GetOrCreate("#01000000");
			}
		}
	}
}
