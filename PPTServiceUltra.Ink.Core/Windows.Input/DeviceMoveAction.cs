namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceMoveAction : DeviceAction
	{
		public event DeviceEventHandler DeviceMove;

		protected override void OnAttached()
		{
			DeviceInput.AddDeviceMoveHandler(AssociatedObject, DeviceMove);
		}

		protected override void OnDetaching()
		{
			DeviceInput.RemoveDeviceMoveHandler(AssociatedObject, DeviceMove);
		}
	}
}
