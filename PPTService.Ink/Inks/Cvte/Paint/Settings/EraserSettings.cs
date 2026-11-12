using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks.Resources;
using Cvte.Windows;

namespace Cvte.Paint.Settings
{
	public class EraserSettings : BoardFilterSettings
	{
		public static Size DefaultEraserSize { get; set; } = new Size(48.0, 72.0);

		public Func<Size, FrameworkElement> CreateEraserVisual { get; set; }

		public EraserSettings()
		{
			CreateEraserVisual = DefaultCreateEraserVisual;
		}

		internal Size GetEraserSize()
		{
			double num = Monitor.PrimaryDevice.PixelBounds.Width / 1920.0 / 1.2;
			Size eraserSizeOfDpi = GetEraserSizeOfDpi();
			return new Size(eraserSizeOfDpi.Width * num / Board.ContentScaleX, eraserSizeOfDpi.Height * num / Board.ContentScaleY);
		}

		private FrameworkElement DefaultCreateEraserVisual(Size touchArea)
		{
			return new EraserView
			{
				Width = touchArea.Width,
				Height = touchArea.Height,
				RenderTransform = new TranslateTransform(),
				IsHitTestVisible = false,
				Cursor = Cursors.None
			};
		}

		private Size GetEraserSizeOfDpi()
		{
			if (Dpi.System.FactorX.Equals(0.0) || Dpi.System.FactorY.Equals(0.0))
			{
				return DefaultEraserSize;
			}
			return new Size(DefaultEraserSize.Width / Dpi.System.FactorX, DefaultEraserSize.Height / Dpi.System.FactorY);
		}
	}
}
