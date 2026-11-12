using Cvte.Paint.Framework;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks.HighPrecision.Erasing.CircleErasing
{
	internal class CircleEraserHandler : HighPrecisionEraserHandler
	{
		private CircleEraserViewManager CircleEraserViewManager { get; }

		private CircleErasingTester CircleErasingTester { get; } = new CircleErasingTester();

		protected override IErasingTester ErasingTester => CircleErasingTester;

		public CircleEraserHandler(int id, Slide currentSlide, EraserSettings eraserSettings, Board board, ErasingSynchronizer erasingSynchronizer)
			: base(id, currentSlide, eraserSettings, board, erasingSynchronizer)
		{
			CircleEraserViewManager = new CircleEraserViewManager(currentSlide, eraserSettings);
		}

		protected override void OnStartEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CircleEraserViewManager.AddToSlide(eraserTouchEventArgs);
		}

		protected override void OnContinueEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CircleEraserViewManager.MoveEraserVisual(ref eraserTouchEventArgs);
			base.OnContinueEraser(eraserTouchEventArgs);
		}

		protected override void OnFinishEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CircleEraserViewManager.RemoveEraserView();
		}
	}
}
