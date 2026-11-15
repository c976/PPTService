namespace PPTService.Paint.Framework.Animations
{
	public interface IAnimationState
	{
		void BackToPreparingState();

		void GoToPlayedState();
	}
}
