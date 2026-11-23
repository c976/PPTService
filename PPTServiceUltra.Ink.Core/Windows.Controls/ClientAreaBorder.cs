using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace PPTServiceUltra.Ink.Core.Windows.Controls
{
	
	
	public class ClientAreaBorder : Border
	{
		private const int SM_CXFRAME = 32;

		private const int SM_CYFRAME = 33;

		private const int SM_CXPADDEDBORDER = 92;

		
		private Window _oldWindow;

		private static Thickness? _paddedBorderThickness;

		private static Thickness? _resizeFrameBorderThickness;

		private static Thickness? _windowChromeNonClientFrameThickness;

		public Thickness PaddedBorderThickness
		{
			get
			{
				Thickness? paddedBorderThickness = _paddedBorderThickness;
				if (!paddedBorderThickness.HasValue)
				{
					int systemMetrics = GetSystemMetrics(92);
					Dpi dpi = GetDpi();
					Size size = new Size(systemMetrics, systemMetrics);
					Size size2 = new Size(size.Width / dpi.FactorX, size.Height / dpi.FactorY);
					_paddedBorderThickness = new Thickness(size2.Width, size2.Height, size2.Width, size2.Height);
				}
				return _paddedBorderThickness.Value;
			}
		}

		public Thickness ResizeFrameBorderThickness
		{
			get
			{
				Thickness valueOrDefault = _resizeFrameBorderThickness.GetValueOrDefault();
				if (!_resizeFrameBorderThickness.HasValue)
				{
					valueOrDefault = new Thickness(SystemParameters.ResizeFrameVerticalBorderWidth, SystemParameters.ResizeFrameHorizontalBorderHeight, SystemParameters.ResizeFrameVerticalBorderWidth, SystemParameters.ResizeFrameHorizontalBorderHeight);
					_resizeFrameBorderThickness = valueOrDefault;
					return valueOrDefault;
				}
				return valueOrDefault;
			}
		}

		public Thickness WindowChromeNonClientFrameThickness
		{
			get
			{
				Thickness thickness = _windowChromeNonClientFrameThickness.GetValueOrDefault();
				if (!_windowChromeNonClientFrameThickness.HasValue)
				{
					thickness = new Thickness(ResizeFrameBorderThickness.Left + PaddedBorderThickness.Left, ResizeFrameBorderThickness.Top + PaddedBorderThickness.Top, ResizeFrameBorderThickness.Right + PaddedBorderThickness.Right, ResizeFrameBorderThickness.Bottom + PaddedBorderThickness.Bottom);
					_windowChromeNonClientFrameThickness = thickness;
					return thickness;
				}
				return thickness;
			}
		}

		[DllImport("user32", ExactSpelling = true)]
		private static extern int GetSystemMetrics(int nIndex);

		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			base.OnVisualParentChanged(oldParent);
			Window oldWindow = _oldWindow;
			if (oldWindow != null)
			{
				oldWindow.StateChanged -= Window_StateChanged;
			}
			Window window = Window.GetWindow(this);
			if (window != null)
			{
				window.StateChanged -= Window_StateChanged;
				window.StateChanged += Window_StateChanged;
			}
			_oldWindow = window;
		}

		private void Window_StateChanged( object sender, EventArgs e)
		{
			Thickness padding = ((((Window)sender).WindowState != WindowState.Maximized) ? default(Thickness) : WindowChromeNonClientFrameThickness);
			Padding = padding;
		}

		private Dpi GetDpi()
		{
			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			if (presentationSource == null)
			{
				return Dpi.System;
			}
			return new Dpi((int)((double)Dpi.ScreenStandard.X * presentationSource.CompositionTarget.TransformToDevice.M11), (int)((double)Dpi.ScreenStandard.Y * presentationSource.CompositionTarget.TransformToDevice.M22));
		}
	}
}
