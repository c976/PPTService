using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMmDeviceEnumerator
	{
		[PreserveSig]
		int EnumAudioEndpoints([In][MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow, [In][MarshalAs(UnmanagedType.U4)] uint stateMask, [MarshalAs(UnmanagedType.Interface)] out IMmDeviceCollection devices);

		[PreserveSig]
		int GetDefaultAudioEndpoint([In][MarshalAs(UnmanagedType.I4)] EDataFlow dataFlow, [In][MarshalAs(UnmanagedType.I4)] ERole role, [MarshalAs(UnmanagedType.Interface)] out IMmDevice device);

		[PreserveSig]
		int GetDevice([In][MarshalAs(UnmanagedType.LPWStr)] string endpointId, [MarshalAs(UnmanagedType.Interface)] out IMmDevice device);

		[PreserveSig]
		int RegisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMmNotificationClient client);

		[PreserveSig]
		int UnregisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMmNotificationClient client);
	}
}
