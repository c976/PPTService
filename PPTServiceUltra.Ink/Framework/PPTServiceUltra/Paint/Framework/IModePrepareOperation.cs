namespace PPTServiceUltra.Paint.Framework
{
	public interface IModePrepareOperation : IModeOperation
	{
		void OnSwitchOn();

		void OnSwitchOff();
	}
}
