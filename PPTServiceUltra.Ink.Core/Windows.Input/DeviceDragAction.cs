namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceDragAction : DeviceAction
	{
		public event DeviceDragEventHandler DeviceDragStarted;

		public event DeviceDragEventHandler DeviceDragDelta;

		public event DeviceDragEventHandler DeviceDragCompleted;

		protected override void OnAttached()
		{
			DeviceInput.AddDeviceDragStartedHandler(AssociatedObject, DeviceDragStarted);
			DeviceInput.AddDeviceDragDeltaHandler(AssociatedObject, DeviceDragDelta);
			DeviceInput.AddDeviceDragCompletedHandler(AssociatedObject, DeviceDragCompleted);
		}

		protected override void OnDetaching()
		{
			DeviceInput.RemoveDeviceDragStartedHandler(AssociatedObject, DeviceDragStarted);
			DeviceInput.RemoveDeviceDragDeltaHandler(AssociatedObject, DeviceDragDelta);
			DeviceInput.RemoveDeviceDragCompletedHandler(AssociatedObject, DeviceDragCompleted);
		}
	}
}
