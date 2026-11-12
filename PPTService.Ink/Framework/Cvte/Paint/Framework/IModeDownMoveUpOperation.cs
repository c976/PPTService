namespace Cvte.Paint.Framework
{
	public interface IModeDownMoveUpOperation : IModeOperation
	{
		void OnDeviceDown(ModeInputEventArgs e);

		void OnDeviceMove(ModeInputEventArgs e);

		void OnDeviceUp(ModeInputEventArgs e);
	}
}
