namespace PPTService.Paint.Framework
{
	internal interface IActiveModeFilter
	{
		Mode CurrentMode { get; set; }

		IActiveModeController ActiveModeController { get; set; }
	}
}
