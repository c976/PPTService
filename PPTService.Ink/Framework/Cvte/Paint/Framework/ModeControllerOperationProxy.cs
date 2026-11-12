namespace Cvte.Paint.Framework
{
	public class ModeControllerOperationProxy
	{
		private readonly Mode _mode;

		public ModeControllerOperationProxy(Mode mode)
		{
			_mode = mode;
		}

		public void SetCurrentSlide(Slide slide)
		{
			_mode.CurrentSlide = slide;
		}

		public void SetHitTestController(IModeHitTestController hitTestController)
		{
			_mode.HitTestController = hitTestController;
		}
	}
}
