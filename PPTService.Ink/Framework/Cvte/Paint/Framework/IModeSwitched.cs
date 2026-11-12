namespace Cvte.Paint.Framework
{
	public interface IModeSwitched
	{
		void OnSwitchedFrom(ModeSwitchedArgs args);

		void OnSwitchedTo(ModeSwitchedArgs args);
	}
}
