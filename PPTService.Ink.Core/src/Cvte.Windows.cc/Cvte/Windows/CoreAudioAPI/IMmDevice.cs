using System;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMmDevice
	{
		[PreserveSig]
		int Activate([In][MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId, [In][MarshalAs(UnmanagedType.U4)] uint classContext, [Optional][In] IntPtr activationParams, [MarshalAs(UnmanagedType.IUnknown)] out object instancePtr);

		[PreserveSig]
		int OpenPropertyStore([In][MarshalAs(UnmanagedType.U4)] uint accessMode, [MarshalAs(UnmanagedType.Interface)] out IPropertyStore properties);

		[PreserveSig]
		int GetId([MarshalAs(UnmanagedType.LPWStr)] out string strId);

		[PreserveSig]
		int GetState([MarshalAs(UnmanagedType.U4)] out uint deviceState);
	}
}
