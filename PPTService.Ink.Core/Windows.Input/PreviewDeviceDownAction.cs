namespace PPTService.Ink.Core.Windows.Input
{
	public class PreviewDeviceDownAction : DeviceAction
	{
		public event DeviceEventHandler PreviewDeviceDown;

		protected override void OnAttached()
		{
			DeviceInput.AddPreviewDeviceDownHandler(AssociatedObject, PreviewDeviceDown);
		}

		protected override void OnDetaching()
		{
			DeviceInput.RemovePreviewDeviceDownHandler(AssociatedObject, PreviewDeviceDown);
		}
	}
}
