namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class PreviewDeviceMoveAction : DeviceAction
	{
		public event DeviceEventHandler PreviewDeviceMove;

		protected override void OnAttached()
		{
			DeviceInput.AddPreviewDeviceMoveHandler(AssociatedObject, PreviewDeviceMove);
		}

		protected override void OnDetaching()
		{
			DeviceInput.RemovePreviewDeviceMoveHandler(AssociatedObject, PreviewDeviceMove);
		}
	}
}
