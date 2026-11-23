using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	[Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMmNotificationClient
	{
		void OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, [MarshalAs(UnmanagedType.U4)] uint newState);

		void OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

		void OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

		void OnDefaultDeviceChanged([MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow, [MarshalAs(UnmanagedType.I4)] ERole deviceRole, [MarshalAs(UnmanagedType.LPWStr)] string defaultDeviceId);

		void OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string deviceId, PROPERTYKEY propertyKey);
	}
}
