using System;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	[Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioEndpointVolume
	{
		[PreserveSig]
		int RegisterControlChangeNotify([In][MarshalAs(UnmanagedType.Interface)] IAudioEndpointVolumeCallback client);

		[PreserveSig]
		int UnregisterControlChangeNotify([In][MarshalAs(UnmanagedType.Interface)] IAudioEndpointVolumeCallback client);

		[PreserveSig]
		int GetChannelCount([MarshalAs(UnmanagedType.U4)] out uint channelCount);

		[PreserveSig]
		int SetMasterVolumeLevel([In][MarshalAs(UnmanagedType.R4)] float level, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int SetMasterVolumeLevelScalar([In][MarshalAs(UnmanagedType.R4)] float level, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int GetMasterVolumeLevel([MarshalAs(UnmanagedType.R4)] out float level);

		[PreserveSig]
		int GetMasterVolumeLevelScalar([MarshalAs(UnmanagedType.R4)] out float level);

		[PreserveSig]
		int SetChannelVolumeLevel([In][MarshalAs(UnmanagedType.U4)] uint channelNumber, [In][MarshalAs(UnmanagedType.R4)] float level, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int SetChannelVolumeLevelScalar([In][MarshalAs(UnmanagedType.U4)] uint channelNumber, [In][MarshalAs(UnmanagedType.R4)] float level, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int GetChannelVolumeLevel([In][MarshalAs(UnmanagedType.U4)] uint channelNumber, [MarshalAs(UnmanagedType.R4)] out float level);

		[PreserveSig]
		int GetChannelVolumeLevelScalar([In][MarshalAs(UnmanagedType.U4)] uint channelNumber, [MarshalAs(UnmanagedType.R4)] out float level);

		[PreserveSig]
		int SetMute([In][MarshalAs(UnmanagedType.Bool)] bool isMuted, [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int GetMute([MarshalAs(UnmanagedType.Bool)] out bool isMuted);

		[PreserveSig]
		int GetVolumeStepInfo([MarshalAs(UnmanagedType.U4)] out uint step, [MarshalAs(UnmanagedType.U4)] out uint stepCount);

		[PreserveSig]
		int VolumeStepUp([In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int VolumeStepDown([In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

		[PreserveSig]
		int QueryHardwareSupport([MarshalAs(UnmanagedType.U4)] out uint hardwareSupportMask);

		[PreserveSig]
		int GetVolumeRange([MarshalAs(UnmanagedType.R4)] out float volumeMin, [MarshalAs(UnmanagedType.R4)] out float volumeMax, [MarshalAs(UnmanagedType.R4)] out float volumeStep);
	}
}
