namespace Cvte.Paint.Framework
{
	public interface IModeDragOperation : IModeOperation
	{
		void OnDragStarted(ModeInputDragEventArgs e);

		void OnDragDelta(ModeInputDragEventArgs e);

		void OnDragCompleted(ModeInputDragEventArgs e);

		void OnDragCanceled(ModeInputDragEventArgs e);
	}
}
