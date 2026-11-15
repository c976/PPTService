namespace PPTService.Ink.Core.Windows.UsbDevice
{
	public class DeviceServiceManager
	{
		public static IUsbDeviceUpdated UsbDeviceUpdated = new UsbDeviceChanged();
	}
}
