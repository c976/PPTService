using System.Security;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public static class WindowParameters
	{
		private static Thickness? _paddedBorderThickness;

		private static Thickness? _resizeFrameBorderThickness;

		private static Thickness? _windowChromeNonClientFrameThickness;

		public static Thickness PaddedBorderThickness
		{
			[SecurityCritical]
			get
			{
				Thickness? paddedBorderThickness = _paddedBorderThickness;
				if (!paddedBorderThickness.HasValue)
				{
					int systemMetrics = Win32.User32.GetSystemMetrics(Win32.SystemMetrics.SM_CXPADDEDBORDER);
					Dpi system = Dpi.System;
					Size size = new Size(systemMetrics, systemMetrics);
					Size size2 = new Size(size.Width / system.FactorX, size.Height / system.FactorY);
					_paddedBorderThickness = new Thickness(size2.Width, size2.Height, size2.Width, size2.Height);
				}
				return _paddedBorderThickness.Value;
			}
		}

		public static Thickness ResizeFrameBorderThickness
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

		public static Thickness WindowChromeNonClientFrameThickness
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
	}
}
