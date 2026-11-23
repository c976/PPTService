using System;
using System.ComponentModel;

namespace PPTServiceUltra.Ink.Core.Windows.UsbDevice
{
	public class DeviceUpdatedEventArgs : EventArgs
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("请使用 UsbUpdatedType 代替，此字段不应该公开", true)]
		public DeviceUpdatedType UsbDeviceUpdatedType;

		public DeviceUpdatedType UsbUpdatedType { get; }

		public Guid? DeviceGuid { get; }

		public string DeviceName { get; }

		public DeviceUpdatedEventArgs(DeviceUpdatedType usbUpdatedType, Guid deviceGuid, string deviceName)
		{
			UsbUpdatedType = usbUpdatedType;
			DeviceGuid = deviceGuid;
			DeviceName = deviceName;
		}

		public DeviceUpdatedEventArgs(DeviceUpdatedType usbUpdatedType)
		{
			UsbUpdatedType = usbUpdatedType;
		}
	}
}
