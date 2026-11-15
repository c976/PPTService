using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace PPTService.Ink.Core.Windows.Devices
{
	
	
	public class DeviceInfo
	{
		
		private static readonly Regex PciRegex = new Regex("^PCI\\\\VEN_([0-9A-F]{4})&DEV_([0-9A-F]{4})", RegexOptions.Compiled);

		
		private static readonly Regex UsbRegex = new Regex("^USB\\\\VID_([0-9A-F]{4})&PID_([0-9A-F]{4})", RegexOptions.Compiled);

		public BusType BusType { get; private set; }

		
		
		public string Description
		{
			
			get;
		}

		
		
		public string InstanceID
		{
			
			get;
		}

		
		
		public string DriverKey
		{
			
			get;
		}

		public string VendorID { get; private set; }

		public string DeviceID { get; private set; }

		public string ProductID { get; private set; }

		
		internal DeviceInfo(string description, string instanceID, string driverKey)
		{
			Description = description;
			InstanceID = instanceID;
			DriverKey = driverKey;
			ParsingInstanceID();
		}

		private void ParsingInstanceID()
		{
			MatchCollection matchCollection = PciRegex.Matches(InstanceID);
			if (matchCollection.Count > 0)
			{
				BusType = BusType.PCI;
				Match match = matchCollection[0];
				VendorID = match.Groups[1].Value;
				DeviceID = match.Groups[2].Value;
				return;
			}
			matchCollection = UsbRegex.Matches(InstanceID);
			if (matchCollection.Count > 0)
			{
				BusType = BusType.USB;
				Match match2 = matchCollection[0];
				VendorID = match2.Groups[1].Value;
				ProductID = match2.Groups[2].Value;
			}
		}

		public bool TryGetDriverVersion(out Version version)
		{
			version = null;
			if (!string.IsNullOrEmpty(DriverKey) && Version.TryParse(Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\" + DriverKey + "\\", "DriverVersion", null) as string, out version))
			{
				return true;
			}
			return false;
		}
	}
}
