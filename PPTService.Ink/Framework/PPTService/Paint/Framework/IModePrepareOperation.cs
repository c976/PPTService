namespace PPTService.Paint.Framework
{
	public interface IModePrepareOperation : IModeOperation
	{
		void OnSwitchOn();

		void OnSwitchOff();
	}
}
