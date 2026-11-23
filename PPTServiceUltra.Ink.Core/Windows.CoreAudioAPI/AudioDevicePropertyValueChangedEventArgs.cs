using System;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	public class AudioDevicePropertyValueChangedEventArgs : EventArgs
	{
		public string DeviceId { get; }

		public PROPERTYKEY PropertyKey { get; }

		public AudioDevicePropertyValueChangedEventArgs(string deviceId, PROPERTYKEY propertyKey)
		{
			DeviceId = deviceId;
			PropertyKey = propertyKey;
		}
	}
}
