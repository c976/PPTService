namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceUpAction : DeviceAction
	{
		public event DeviceEventHandler DeviceUp;

		protected override void OnAttached()
		{
			DeviceInput.AddDeviceUpHandler(AssociatedObject, DeviceUp);
		}

		protected override void OnDetaching()
		{
			DeviceInput.RemoveDeviceUpHandler(AssociatedObject, DeviceUp);
		}
	}
}
