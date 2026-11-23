using System;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	public class DefaultAudioDeviceChangedEventArgs : EventArgs
	{
		public EDataFlow DataFlow { get; }

		public ERole DeviceRole { get; }

		public string DefaultDeviceId { get; }

		public DefaultAudioDeviceChangedEventArgs(EDataFlow dataFlow, ERole deviceRole, string defaultDeviceId)
		{
			DataFlow = dataFlow;
			DeviceRole = deviceRole;
			DefaultDeviceId = defaultDeviceId;
		}
	}
}
