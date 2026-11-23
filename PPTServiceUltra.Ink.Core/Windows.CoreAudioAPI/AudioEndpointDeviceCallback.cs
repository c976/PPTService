using System;

namespace PPTServiceUltra.Ink.Core.Windows.CoreAudioAPI
{
	public class AudioEndpointDeviceCallback
	{
		public event EventHandler<DefaultAudioDeviceChangedEventArgs> DefaultDeviceChanged;

		public event EventHandler<AudioDeviceAddedEventArgs> DeviceAdded;

		public event EventHandler<AudioDeviceRemovedEventArgs> DeviceRemoved;

		public event EventHandler<AudioDeviceStateChangedEventArgs> DeviceStateChanged;

		public event EventHandler<AudioDevicePropertyValueChangedEventArgs> PropertyValueChanged;

		internal void OnDefaultDeviceChanged(DefaultAudioDeviceChangedEventArgs args)
		{
			DefaultDeviceChanged?.Invoke(this, args);
		}

		internal void OnDeviceAdded(AudioDeviceAddedEventArgs args)
		{
			DeviceAdded?.Invoke(this, args);
		}

		internal void OnDeviceRemoved(AudioDeviceRemovedEventArgs args)
		{
			DeviceRemoved?.Invoke(this, args);
		}

		internal void OnDeviceStateChanged(AudioDeviceStateChangedEventArgs args)
		{
			DeviceStateChanged?.Invoke(this, args);
		}

		internal void OnPropertyValueChanged(AudioDevicePropertyValueChangedEventArgs args)
		{
			PropertyValueChanged?.Invoke(this, args);
		}
	}
}
