using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	public class SynchronousSelectEraseMode : CompositeMode, IEraserMode
	{
		public SynchronousSelectEraseMode()
		{
			ModeOperations.Add(new SynchronousSelectionEraseModeOperation());
		}
	}
}
