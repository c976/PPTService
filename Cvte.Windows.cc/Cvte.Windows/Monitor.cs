using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Cvte.Windows
{
	public class Monitor
	{
		private static readonly Lazy<MonitorCollection> AllDevicesLazy = new Lazy<MonitorCollection>(() =>
		{
			SystemEvents.DisplaySettingsChanged += (object sender, EventArgs e) =>
			{
				UpdateAllDevices();
			};
			return new MonitorCollection(Screen.AllScreens.Select((Screen x) => new MonitorDevice(x)));
		}, LazyThreadSafetyMode.ExecutionAndPublication);

		public static MonitorCollection Devices => AllDevicesLazy.Value;

		public static MonitorDevice PrimaryDevice => Devices.Primary;

		public static bool IsPerMonitorDpiSupported
		{
			get
			{
				Version version = Environment.OSVersion.Version;
				if (version.Major < 10)
				{
					if (version.Major == 6)
					{
						return version.Minor == 3;
					}
					return false;
				}
				return true;
			}
		}

		internal static MonitorDevice FromWin32Screen(Screen screen)
		{
			return Devices.First((MonitorDevice device) => object.Equals(device.Win32Screen, screen));
		}

		private static void UpdateAllDevices()
		{
			Devices.Update(Screen.AllScreens.Select((Screen x) => new MonitorDevice(x)));
		}
	}
}
