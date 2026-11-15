using PPTService.Paint.Framework;
using PPTService.Paint.Settings;

namespace PPTService.Paint.Inks.HighPrecision.Erasing.CornerRadiusRectangleErasing
{
	public class CornerRadiusRectangleEraserHandler : HighPrecisionEraserHandler
	{
		private CornerRadiusRectangleEraserViewManager CornerRadiusRectangleEraserViewManager { get; }

		private RectangleErasingTester RectangleErasingTester { get; } = new RectangleErasingTester();

		protected override IErasingTester ErasingTester => RectangleErasingTester;

		public CornerRadiusRectangleEraserHandler(int id, Slide currentSlide, EraserSettings eraserSettings, Board board, ErasingSynchronizer erasingSynchronizer)
			: base(id, currentSlide, eraserSettings, board, erasingSynchronizer)
		{
			CornerRadiusRectangleEraserViewManager = new CornerRadiusRectangleEraserViewManager(currentSlide, eraserSettings);
		}

		private void DrawCore(DebuggablePathInfo path)
		{
		}

		protected override void OnStartEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CornerRadiusRectangleEraserViewManager.AddToSlide(eraserTouchEventArgs);
		}

		protected override void OnContinueEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CornerRadiusRectangleEraserViewManager.MoveEraserVisual(ref eraserTouchEventArgs);
			base.OnContinueEraser(eraserTouchEventArgs);
		}

		protected override void OnFinishEraser(EraserTouchEventArgs eraserTouchEventArgs)
		{
			CornerRadiusRectangleEraserViewManager.RemoveEraserView();
		}
	}
}
