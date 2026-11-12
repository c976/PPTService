using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cvte.Paint.Framework;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	[Obsolete("解决依赖之后删除这个类")]
	public class EraserAdornerRender
	{
		private FrameworkElement _eraserVisual;

		private readonly EraserSettings _eraserSettings;

		public Slide CurrentSlide { get; private set; }

		public Size EraserSize { get; set; }

		public EraserAdornerRender(Slide slide)
		{
			CurrentSlide = slide;
			_eraserSettings = new EraserSettings();
			EraserSize = _eraserSettings.GetEraserSize();
		}

		public void RemoveEraser()
		{
			if (_eraserVisual != null)
			{
				CurrentSlide.RemoveUtilities(_eraserVisual);
				_eraserVisual = null;
			}
		}

		internal void MoveEraserVisual(Point point)
		{
			if (_eraserVisual == null)
			{
				_eraserVisual = _eraserSettings.CreateEraserVisual(EraserSize);
				_eraserVisual.Opacity = 0.0;
				_eraserVisual.Width = EraserSize.Width;
				_eraserVisual.Height = EraserSize.Height;
				_eraserVisual.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(200.0))));
				CurrentSlide.AddUtilities(_eraserVisual);
			}
			if (!object.Equals(_eraserVisual.Width, EraserSize.Width) || !object.Equals(_eraserVisual.Height, EraserSize.Height))
			{
				_eraserVisual.Width = EraserSize.Width;
				_eraserVisual.Height = EraserSize.Height;
			}
			TranslateTransform translateTransform = (TranslateTransform)_eraserVisual.RenderTransform;
			double num = ((Math.Abs(_eraserVisual.ActualWidth) < 0.0001) ? _eraserVisual.Width : _eraserVisual.ActualWidth);
			double num2 = ((Math.Abs(_eraserVisual.ActualHeight) < 0.0001) ? _eraserVisual.Height : _eraserVisual.ActualHeight);
			translateTransform.X = point.X - num / 2.0;
			translateTransform.Y = point.Y - num2 / 2.0;
		}
	}
}
