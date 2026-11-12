using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Microsoft.Win32;

namespace PPTService.Core.Net
{
	
	
	internal static class EnvironmentUtility
	{
		
		private static string _macId;

		private static bool IsWindowsManagementInstrumentationAvailable
		{
			get
			{
				try
				{
					using (ServiceController serviceController = new ServiceController("Winmgmt"))
					{
						return serviceController.Status == ServiceControllerStatus.Running;
					}
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public static string MacAddressId
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_macId))
				{
					return _macId;
				}
				try
				{
					if (!IsWindowsManagementInstrumentationAvailable)
					{
						return string.Empty;
					}
					foreach (ManagementObject item in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances().OfType<ManagementObject>())
					{
						using (item)
						{
							if ((bool)item["IPEnabled"])
							{
								if (item["MacAddress"] == null)
								{
									return string.Empty;
								}
								_macId = item["MacAddress"].ToString().ToUpper();
								return _macId;
							}
						}
					}
					return string.Empty;
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}

		public static string CPU
		{
			get
			{
				if (!IsWindowsManagementInstrumentationAvailable)
				{
					return string.Empty;
				}
				return GetCpuInfo();
			}
		}

		public static string RAM
		{
			get
			{
				try
				{
					if (!IsWindowsManagementInstrumentationAvailable)
					{
						return string.Empty;
					}
					ulong totalMemoryInKilobytes;
					if (GetPhysicallyInstalledSystemMemory(out totalMemoryInKilobytes))
					{
						return ((double)totalMemoryInKilobytes / 1024.0 / 1024.0).ToString("F1") + "G";
					}
					return string.Empty;
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}

		public static string ScreenType
		{
			get
			{
				try
				{
					if (!IsWindowsManagementInstrumentationAvailable)
					{
						return string.Empty;
					}
					using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor"))
					{
						using (IEnumerator<ManagementObject> enumerator = managementObjectSearcher.Get().OfType<ManagementObject>().GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								return Convert.ToString(enumerator.Current["Description"]);
							}
						}
					}
					return string.Empty;
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}

		public static string OSbit
		{
			get
			{
				if (!Environment.Is64BitOperatingSystem)
				{
					return "x86";
				}
				return "x64";
			}
		}

		public static string TimeZone
		{
			get
			{
				try
				{
					short num = Convert.ToInt16(TimeZoneInfo.Local.BaseUtcOffset.TotalHours);
					if (num > 0)
					{
						return "+" + num;
					}
					return num.ToString();
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}

		public static string NdpVersion
		{
			get
			{
				try
				{
					return NdpInfo.GetCurrentVersionName() ?? string.Empty;
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}

		private static string GetCpuInfo()
		{
			return Registry.GetValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "ProcessorNameString", string.Empty)?.ToString() ?? string.Empty;
		}

		[DllImport("Kernel32.dll")]
		private static extern bool GetPhysicallyInstalledSystemMemory(out ulong totalMemoryInKilobytes);
	}
}
