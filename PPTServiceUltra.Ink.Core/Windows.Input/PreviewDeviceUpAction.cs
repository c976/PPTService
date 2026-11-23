namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class PreviewDeviceUpAction : DeviceAction
	{
		public event DeviceEventHandler PreviewDeviceUp;

		protected override void OnAttached()
		{
			DeviceInput.AddPreviewDeviceUpHandler(AssociatedObject, PreviewDeviceUp);
		}

		protected override void OnDetaching()
		{
			DeviceInput.RemovePreviewDeviceUpHandler(AssociatedObject, PreviewDeviceUp);
		}
	}
}
