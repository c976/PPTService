namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceClickAction : DeviceAction
	{
		public event DeviceEventHandler DeviceClick;

		protected override void OnAttached()
		{
			DeviceInput.AddDeviceClickHandler(AssociatedObject, DeviceClick);
		}

		protected override void OnDetaching()
		{
			DeviceInput.RemoveDeviceClickHandler(AssociatedObject, DeviceClick);
		}
	}
}
