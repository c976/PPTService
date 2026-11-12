namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public interface IHighPrecisionEraserHandler
	{
		void StartEraser(EraserTouchEventArgs eraserTouchEventArgs);

		void ContinueEraser(EraserTouchEventArgs eraserTouchEventArgs);

		void FinishEraser(EraserTouchEventArgs eraserTouchEventArgs);

		void CompleteEraser();
	}
}
