using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Cvte.Net.WebServices.Myou.Sunday
{
	
	
	internal class SundayEnvironment
	{
		private string _osName;

		private string _osVersion;

		private string _osLang;

		private string _ram;

		private string _cpu;

		private string _mac;

		private string _deviceGraph;

		private string _deviceId;

		private string _screenResolution;

		public string OSName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_osName))
				{
					_osName = Environment.OSVersion.VersionString;
				}
				return _osName;
			}
		}

		public string OSVersion
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_osVersion))
				{
					_osVersion = Environment.OSVersion.Version.ToString();
				}
				return _osVersion;
			}
		}

		public string OSLang
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_osLang))
				{
					_osLang = CultureInfo.InstalledUICulture.Name;
				}
				return _osLang;
			}
		}

		public string Ram
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_ram))
				{
					_ram = EnvironmentUtility.RAM;
				}
				return _ram;
			}
		}

		public string Cpu
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_cpu))
				{
					_cpu = EnvironmentUtility.CPU;
				}
				return _cpu;
			}
		}

		public string Mac
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_mac))
				{
					_mac = EnvironmentUtility.MacAddressId;
				}
				return _mac;
			}
		}

		public string DeviceGraph
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_deviceGraph))
				{
					_deviceGraph = EnvironmentUtility.ScreenType;
				}
				return _deviceGraph;
			}
		}

		public string DeviceId
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_deviceId))
				{
					_deviceId = SecurityUtility.Encrypt32(EnvironmentUtility.MacAddressId);
				}
				return _deviceId;
			}
		}

		public string ScreenResolution
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_screenResolution))
				{
					_screenResolution = $"{SystemParameters.PrimaryScreenWidth}*{SystemParameters.PrimaryScreenHeight}";
				}
				return _screenResolution;
			}
		}
	}
}
