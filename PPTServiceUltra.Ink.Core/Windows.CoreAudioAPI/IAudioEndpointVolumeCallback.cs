using System;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	[Guid("657804FA-D6AD-4496-8A60-352752AF4F89")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioEndpointVolumeCallback
	{
		[PreserveSig]
		int OnNotify([In] IntPtr notificationData);
	}
}
