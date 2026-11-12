using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	public class SynchronousSelectEraseMode : CompositeMode, IEraserMode
	{
		public SynchronousSelectEraseMode()
		{
			ModeOperations.Add(new SynchronousSelectionEraseModeOperation());
		}
	}
}
