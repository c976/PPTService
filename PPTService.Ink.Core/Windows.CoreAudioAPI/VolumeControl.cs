using System;

namespace PPTService.Ink.Core.Windows.CoreAudioAPI
{
	public class VolumeControl : IDisposable
	{
		private IAudioEndpointVolume _currentAudioEndpoint;

		private readonly IMmDeviceEnumerator _mmDeviceEnumerator;

		private readonly AudioEndpointVolumeCallback _audioEndpointVolumeCallback;

		private readonly MmNotificationClient _mmNotificationClient;

		public bool IsMuted
		{
			get
			{
				bool isMuted;
				_currentAudioEndpoint.GetMute(out isMuted);
				return isMuted;
			}
			set
			{
				_currentAudioEndpoint.SetMute(value, Guid.NewGuid());
			}
		}

		public VolumeControl(AudioEndpointVolumeCallback audioEndpointVolumeCallback, AudioEndpointDeviceCallback audioEndpointDeviceCallback)
		{
			if (audioEndpointDeviceCallback == null)
			{
				throw new ArgumentNullException("audioEndpointDeviceCallback");
			}
			if (audioEndpointVolumeCallback == null)
			{
				throw new ArgumentNullException("audioEndpointVolumeCallback");
			}
			_audioEndpointVolumeCallback = audioEndpointVolumeCallback;
			_mmNotificationClient = new MmNotificationClient(audioEndpointDeviceCallback);
			_mmNotificationClient.DefaultDeviceChanged += MmNotificationClientOnDefaultDeviceChanged;
			Type typeFromCLSID = Type.GetTypeFromCLSID(new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E"));
			_mmDeviceEnumerator = (IMmDeviceEnumerator)Activator.CreateInstance(typeFromCLSID);
			_mmDeviceEnumerator.RegisterEndpointNotificationCallback(_mmNotificationClient);
			IMmDevice device;
			_mmDeviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eConsole, out device);
			UpdateAudioEndpointVolume(device);
		}

		private void UpdateAudioEndpointVolume(IMmDevice device)
		{
			Guid interfaceId = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
			object instancePtr;
			device.Activate(interfaceId, 1u, IntPtr.Zero, out instancePtr);
			_currentAudioEndpoint = instancePtr as IAudioEndpointVolume;
			_currentAudioEndpoint?.UnregisterControlChangeNotify(_audioEndpointVolumeCallback);
			_currentAudioEndpoint?.RegisterControlChangeNotify(_audioEndpointVolumeCallback);
		}

		private void MmNotificationClientOnDefaultDeviceChanged(object sender, string defaultDeviceId)
		{
			IMmDevice device;
			_mmDeviceEnumerator.GetDevice(defaultDeviceId, out device);
			UpdateAudioEndpointVolume(device);
		}

		public void SetVolume(int volume)
		{
			float level;
			if (volume >= 100)
			{
				level = 1f;
			}
			else
			{
				level = ((volume < 0) ? 0f : ((float)volume / 100f));
			}
			_currentAudioEndpoint.SetMasterVolumeLevelScalar(level, Guid.NewGuid());
		}

		public int GetVolume()
		{
			float level;
			_currentAudioEndpoint.GetMasterVolumeLevelScalar(out level);
			return (int)Math.Round(level * 100f);
		}

		public void Dispose()
		{
			_mmDeviceEnumerator.UnregisterEndpointNotificationCallback(_mmNotificationClient);
			_currentAudioEndpoint?.UnregisterControlChangeNotify(_audioEndpointVolumeCallback);
		}
	}
}
