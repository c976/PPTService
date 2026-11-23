using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	[Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMmDeviceCollection
	{
		[PreserveSig]
		int GetCount([MarshalAs(UnmanagedType.U4)] out uint count);

		[PreserveSig]
		int Item([In][MarshalAs(UnmanagedType.U4)] uint index, [MarshalAs(UnmanagedType.Interface)] out IMmDevice device);
	}
}
