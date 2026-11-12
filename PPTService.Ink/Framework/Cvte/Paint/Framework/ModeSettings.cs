using System.ComponentModel;

namespace Cvte.Paint.Framework
{
	public class ModeSettings
	{
		public bool IsTouchCursorVisible { get; set; } = true;

		[DefaultValue(true)]
		public bool IsElementInteractionEnabled { get; set; } = true;

		public ModeSettings()
		{
		}

		public ModeSettings(bool isTouchCursorVisible = true, bool isElementInteractionEnabled = true)
		{
			IsTouchCursorVisible = isTouchCursorVisible;
			IsElementInteractionEnabled = isElementInteractionEnabled;
		}
	}
}
