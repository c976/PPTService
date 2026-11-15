using System;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	internal class MmNotificationClient : IMmNotificationClient
	{
		private readonly AudioEndpointDeviceCallback _audioEndpointDeviceCallback;

		public event EventHandler<string> DefaultDeviceChanged;

		internal MmNotificationClient(AudioEndpointDeviceCallback audioEndpointDeviceCallback)
		{
			if (audioEndpointDeviceCallback == null)
			{
				throw new ArgumentNullException("audioEndpointDeviceCallback");
			}
			_audioEndpointDeviceCallback = audioEndpointDeviceCallback;
		}

		public void OnDeviceStateChanged(string deviceId, uint newState)
		{
			_audioEndpointDeviceCallback.OnDeviceStateChanged(new AudioDeviceStateChangedEventArgs(deviceId, newState));
		}

		public void OnDeviceAdded(string deviceId)
		{
			_audioEndpointDeviceCallback.OnDeviceAdded(new AudioDeviceAddedEventArgs(deviceId));
		}

		public void OnDeviceRemoved(string deviceId)
		{
			_audioEndpointDeviceCallback.OnDeviceRemoved(new AudioDeviceRemovedEventArgs(deviceId));
		}

		public void OnDefaultDeviceChanged(EDataFlow dataFlow, ERole deviceRole, string defaultDeviceId)
		{
			DefaultDeviceChanged?.Invoke(this, defaultDeviceId);
			_audioEndpointDeviceCallback.OnDefaultDeviceChanged(new DefaultAudioDeviceChangedEventArgs(dataFlow, deviceRole, defaultDeviceId));
		}

		public void OnPropertyValueChanged(string deviceId, PROPERTYKEY propertyKey)
		{
			_audioEndpointDeviceCallback.OnPropertyValueChanged(new AudioDevicePropertyValueChangedEventArgs(deviceId, propertyKey));
		}
	}
}
