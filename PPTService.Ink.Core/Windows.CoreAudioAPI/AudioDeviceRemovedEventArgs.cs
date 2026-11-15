using System;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	public class AudioDeviceRemovedEventArgs : EventArgs
	{
		public string DeviceId { get; }

		public AudioDeviceRemovedEventArgs(string deviceId)
		{
			DeviceId = deviceId;
		}
	}
}
