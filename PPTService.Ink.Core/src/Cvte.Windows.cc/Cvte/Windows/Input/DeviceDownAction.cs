namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceDownAction : DeviceAction
	{
		public event DeviceEventHandler DeviceDown;

		protected override void OnAttached()
		{
			DeviceInput.AddDeviceDownHandler(AssociatedObject, DeviceDown);
		}

		protected override void OnDetaching()
		{
			DeviceInput.RemoveDeviceDownHandler(AssociatedObject, DeviceDown);
		}
	}
}
