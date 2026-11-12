namespace Cvte.Paint.Framework
{
	public interface IModeController
	{
		Mode CurrentMode { get; set; }

		void SwitchOn();

		void SwitchOff();
	}
}
