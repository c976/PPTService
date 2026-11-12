using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cvte.Paint.Framework;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	internal class CornerRadiusRectangleEraserViewManager
	{
		public Slide CurrentSlide { get; }

		public EraserSettings EraserSettings { get; }

		public CornerRadiusRectangleEraserView CornerRadiusRectangleEraserView { get; set; }

		public CornerRadiusRectangleEraserViewManager(Slide currentSlide, EraserSettings eraserSettings)
		{
			CurrentSlide = currentSlide;
			EraserSettings = eraserSettings;
		}

		public void AddToSlide(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CornerRadiusRectangleEraserView = new CornerRadiusRectangleEraserView();
			MoveEraserVisual(ref eraserTouchEventArgs);
			CurrentSlide.AddUtilities(CornerRadiusRectangleEraserView);
		}

		public void MoveEraserVisual([In]ref EraserTouchEventArgs eraserTouchEventArgs)
		{
			if (CornerRadiusRectangleEraserView == null)
			{
				AddToSlide(eraserTouchEventArgs);
			}
			CornerRadiusRectangleEraserView.X = eraserTouchEventArgs.X - eraserTouchEventArgs.Width / 2.0;
			CornerRadiusRectangleEraserView.Y = eraserTouchEventArgs.Y - eraserTouchEventArgs.Height / 2.0;
			CornerRadiusRectangleEraserView.Width = eraserTouchEventArgs.Width;
			CornerRadiusRectangleEraserView.Height = eraserTouchEventArgs.Height;
			CornerRadiusRectangleEraserView.Rotation = eraserTouchEventArgs.Rotation;
		}

		public void RemoveEraserView()
		{
			CurrentSlide.RemoveUtilities(CornerRadiusRectangleEraserView);
		}
	}
}
