using PPTServiceUltra.Paint.Framework;

namespace PPTServiceUltra.Paint.Inks
{
	public class SynchronousSelectEraseMode : CompositeMode, IEraserMode
	{
		public SynchronousSelectEraseMode()
		{
			ModeOperations.Add(new SynchronousSelectionEraseModeOperation());
		}
	}
}
