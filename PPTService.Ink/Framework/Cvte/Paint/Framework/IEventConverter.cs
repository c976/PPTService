namespace Cvte.Paint.Framework
{
	public interface IEventConverter
	{
		Slide CurrentSlide { get; set; }

		void InputStarting(ModeInputEventArgs e);

		void InputCompleted(ModeInputEventArgs e);

		void DeviceDown(ModeInputEventArgs e);

		void DeviceMove(ModeInputEventArgs e);

		void DeviceUp(ModeInputEventArgs e);

		void SwitchOn();

		void SwitchOff();
	}
}
