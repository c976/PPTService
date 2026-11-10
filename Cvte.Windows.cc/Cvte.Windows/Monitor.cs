using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Cvte.Windows
{
	public class Monitor
	{
		private static readonly Lazy<MonitorCollection> AllDevicesLazy;

		private static Func<MonitorCollection> CS_0024_003C_003E9__CachedAnonymousMethodDelegate8;

		private static EventHandler CS_0024_003C_003E9__CachedAnonymousMethodDelegate9;

		public static MonitorCollection Devices => AllDevicesLazy.Value;

		public static MonitorDevice PrimaryDevice => Devices.Primary;

		public static bool IsPerMonitorDpiSupported
		{
			get
			{
				int num = 0;
				if (0 == 0)
				{
					Version version;
					if (0 == 0)
					{
						version = Environment.OSVersion.Version;
					}
					if (7 != 0)
					{
						if (version.Major >= 10)
						{
							goto IL_0028;
						}
						if (version.Major == 6)
						{
							num = version.Minor;
							goto IL_0022;
						}
					}
					return false;
				}
				goto IL_0028;
				IL_0022:
				return num == 3;
				IL_0028:
				num = 1;
				if (num != 0)
				{
					return (byte)num != 0;
				}
				goto IL_0022;
			}
		}

		internal static MonitorDevice FromWin32Screen(Screen screen)
		{
			while (false)
			{
			}
			return Devices.First((MonitorDevice device) => object.Equals(device.Win32Screen, screen));
		}

		private static void UpdateAllDevices()
		{
			do
			{
				if (0 == 0)
				{
					Devices.Update(Screen.AllScreens.Select((Screen x) => new MonitorDevice(x)));
				}
			}
			while (false);
		}

		static Monitor()
		{
			if (true && (false || CS_0024_003C_003E9__CachedAnonymousMethodDelegate8 == null))
			{
				goto IL_000d;
			}
			goto IL_001e;
			IL_000d:
			CS_0024_003C_003E9__CachedAnonymousMethodDelegate8 = () =>
			{
				while (true)
				{
					if (true && CS_0024_003C_003E9__CachedAnonymousMethodDelegate9 == null)
					{
						goto IL_000a;
					}
					goto IL_001b;
					IL_000a:
					CS_0024_003C_003E9__CachedAnonymousMethodDelegate9 = (object sender, EventArgs args) =>
					{
						UpdateAllDevices();
					};
					goto IL_001b;
					IL_001b:
					if (false || 7 == 0)
					{
						continue;
					}
					if (0 == 0)
					{
						break;
					}
					goto IL_000a;
				}
				SystemEvents.DisplaySettingsChanged += CS_0024_003C_003E9__CachedAnonymousMethodDelegate9;
				return new MonitorCollection(Screen.AllScreens.Select((Screen x) => new MonitorDevice(x)));
			};
			goto IL_001e;
			IL_001e:
			AllDevicesLazy = new Lazy<MonitorCollection>(CS_0024_003C_003E9__CachedAnonymousMethodDelegate8, LazyThreadSafetyMode.ExecutionAndPublication);
			if (0 == 0 && 0 == 0)
			{
				return;
			}
			goto IL_000d;
		}
	}
}
