using System;

namespace PPTService.Ink.Core.Windows.UsbDevice
{
	internal class UsbDeviceInfo
	{
		public struct DevBroadcastDeviceinterface
		{
			internal int Size;

			internal int DeviceType;

			internal int Reserved;

			internal Guid ClassGuid;

			internal short Name;
		}

		public const int WM_DEVICE_CHANGE = 537;

		public const int DBT_DEVICE_ARRIVAL = 32768;

		public const int DBT_DEVICE_REMOVE_COMPLETE = 32772;

		public const int DBT_DEVNODES_CHANGED = 7;

		public const int DBT_DEVTYPE_DEVICE_INTERFACE = 5;

		public static readonly Guid GuidDevInterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
	}
}
