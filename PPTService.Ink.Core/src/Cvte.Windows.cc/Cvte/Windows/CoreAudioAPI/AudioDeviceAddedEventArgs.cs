using System;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	public class AudioDeviceAddedEventArgs : EventArgs
	{
		public string DeviceId { get; }

		public AudioDeviceAddedEventArgs(string deviceId)
		{
			DeviceId = deviceId;
		}
	}
}
