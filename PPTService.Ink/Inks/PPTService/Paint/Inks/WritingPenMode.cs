using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	public abstract class WritingPenMode : Mode, IPenMode
	{
		protected WritingPenMode()
			: base(new ModeSettings
			{
				IsElementInteractionEnabled = false,
				IsTouchCursorVisible = false
			})
		{
		}

		protected WritingPenMode(ModeSettings modeSettings)
			: base(modeSettings)
		{
		}
	}
}
