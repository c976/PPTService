namespace Cvte.Paint.Framework
{
	public interface IOneTimeMode
	{
		bool IsActive { get; }

		bool AutoSwitchOff { get; set; }

		void EndOneTimeMode();
	}
}
