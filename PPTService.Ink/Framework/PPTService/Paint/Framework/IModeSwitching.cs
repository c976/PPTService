namespace PPTService.Paint.Framework
{
	public interface IModeSwitching
	{
		void OnSwitchingFrom(ModeSwitchingArgs args);

		void OnSwitchingTo(ModeSwitchingArgs args);
	}
}
