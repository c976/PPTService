using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cvte.Paint.Framework;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing
{
	internal class CircleEraserViewManager
	{
		public Slide CurrentSlide { get; }

		public EraserSettings EraserSettings { get; }

		public CircleEraserView EraserView { get; set; }

		public CircleEraserViewManager(Slide currentSlide, EraserSettings eraserSettings)
		{
			CurrentSlide = currentSlide;
			EraserSettings = eraserSettings;
		}

		public void AddToSlide(EraserTouchEventArgs eraserTouchEventArgs)
		{
			EraserView = new CircleEraserView();
			MoveEraserVisual(ref eraserTouchEventArgs);
			CurrentSlide.AddUtilities(EraserView);
		}

		public void MoveEraserVisual([In]ref EraserTouchEventArgs eraserTouchEventArgs)
		{
			if (EraserView == null)
			{
				AddToSlide(eraserTouchEventArgs);
			}
			EraserView.Width = eraserTouchEventArgs.Width;
			EraserView.Height = eraserTouchEventArgs.Width;
			double x = eraserTouchEventArgs.Position.X - eraserTouchEventArgs.Width / 2.0;
			double y = eraserTouchEventArgs.Position.Y - eraserTouchEventArgs.Width / 2.0;
			EraserView.X = x;
			EraserView.Y = y;
		}

		public void RemoveEraserView()
		{
			CurrentSlide.RemoveUtilities(EraserView);
		}
	}
}
