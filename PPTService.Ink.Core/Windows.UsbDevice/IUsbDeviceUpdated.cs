using System;
using System.Windows;

namespace PPTService.Ink.Core.Windows.UsbDevice
{
	public interface IUsbDeviceUpdated
	{
		event EventHandler<DeviceUpdatedEventArgs> DeviceUpdated;

		bool AddDeviceChangedHook(Window window);

		int RemoveDeviceChangeHook();
	}
}
