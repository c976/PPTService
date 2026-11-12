namespace Cvte.Paint.Framework
{
	public interface IModePrepareOperation : IModeOperation
	{
		void OnSwitchOn();

		void OnSwitchOff();
	}
}
