using System;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.UsbDevice
{
	public interface IUsbDeviceUpdated
	{
		event EventHandler<DeviceUpdatedEventArgs> DeviceUpdated;

		bool AddDeviceChangedHook(Window window);

		int RemoveDeviceChangeHook();
	}
}
