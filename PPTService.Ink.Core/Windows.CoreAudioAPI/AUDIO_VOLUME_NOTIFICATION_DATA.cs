using System;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct AUDIO_VOLUME_NOTIFICATION_DATA
	{
		public Guid EventContext;

		[MarshalAs(UnmanagedType.Bool)]
		public bool IsMuted;

		[MarshalAs(UnmanagedType.R4)]
		public float MasterVolume;

		[MarshalAs(UnmanagedType.U4)]
		public uint ChannelCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.R4)]
		public float[] ChannelVolumes;
	}
}
