using System;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	public class AudioDeviceStateChangedEventArgs : EventArgs
	{
		public string DeviceId { get; }

		public uint NewState { get; }

		public AudioDeviceStateChangedEventArgs(string deviceId, uint newState)
		{
			DeviceId = deviceId;
			NewState = newState;
		}
	}
}
