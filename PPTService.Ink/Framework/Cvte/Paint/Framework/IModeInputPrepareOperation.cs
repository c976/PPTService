namespace Cvte.Paint.Framework
{
	public interface IModeInputPrepareOperation : IModeOperation
	{
		void OnInputStarting(ModeInputEventArgs e);

		void OnInputCompleted(ModeInputEventArgs e);
	}
}
