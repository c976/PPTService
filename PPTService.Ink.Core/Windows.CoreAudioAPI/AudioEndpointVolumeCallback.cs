using System;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	public class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
	{
		public int CurrentVolume { get; private set; }

		public bool IsMuted { get; private set; }

		public event EventHandler VolumeStatChange;

		public int OnNotify(IntPtr dataPtr)
		{
			AUDIO_VOLUME_NOTIFICATION_DATA aUDIO_VOLUME_NOTIFICATION_DATA = (AUDIO_VOLUME_NOTIFICATION_DATA)Marshal.PtrToStructure(dataPtr, typeof(AUDIO_VOLUME_NOTIFICATION_DATA));
			CurrentVolume = int.Parse((aUDIO_VOLUME_NOTIFICATION_DATA.MasterVolume * 100f).ToString("0"));
			IsMuted = aUDIO_VOLUME_NOTIFICATION_DATA.IsMuted;
			VolumeStatChange?.Invoke(this, EventArgs.Empty);
			return 0;
		}
	}
}
