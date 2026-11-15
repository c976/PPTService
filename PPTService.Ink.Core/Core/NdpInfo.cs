using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PPTService.Ink.Core
{
	
	
	public sealed class NdpInfo
	{
		private static readonly Dictionary<int, string> ReleaseToNameDictionary = new Dictionary<int, string>
		{
			{ 378389, "4.5" },
			{ 378675, "4.5.1" },
			{ 378758, "4.5.1" },
			{ 379893, "4.5.2" },
			{ 393295, "4.6" },
			{ 393297, "4.6" },
			{ 394254, "4.6.1" },
			{ 394271, "4.6.1" },
			{ 394802, "4.6.2" },
			{ 394806, "4.6.2" },
			{ 460798, "4.7" },
			{ 460805, "4.7" },
			{ 461308, "4.7.1" },
			{ 461310, "4.7.1" },
			{ 461808, "4.7.2" },
			{ 461814, "4.7.2" },
			{ 528040, "4.8" },
			{ 528049, "4.8" }
		};

		public string MainVersion { get; }

		
		
		public string ReleaseVersionName
		{
			
			get;
		}

		public string Version { get; }

		public int Release { get; }

		public int SP { get; }

		public string DisplayVersion
		{
			get
			{
				string value;
				if (ReleaseToNameDictionary.TryGetValue(Release, out value))
				{
					return value;
				}
				Version version = new Version(Version);
				return $"{version.Major}.{version.Minor}";
			}
		}

		
		public string DisplayReleaseVersionName
		{
			
			get
			{
				if (string.IsNullOrWhiteSpace(ReleaseVersionName) || ReleaseVersionName == "Full")
				{
					return "";
				}
				if (ReleaseVersionName == "Client")
				{
					return "Client Profile";
				}
				return ReleaseVersionName;
			}
		}

		private NdpInfo(string mainVersion,  string releaseVersionName, string version, int release, int sp)
		{
			MainVersion = mainVersion;
			ReleaseVersionName = releaseVersionName;
			Version = version;
			Release = release;
			SP = sp;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(".NET Framework " + DisplayVersion);
			if (SP > 0)
			{
				stringBuilder.Append($" SP{SP}");
			}
			string displayReleaseVersionName = DisplayReleaseVersionName;
			if (!string.IsNullOrWhiteSpace(displayReleaseVersionName))
			{
				stringBuilder.Append(" " + displayReleaseVersionName);
			}
			return stringBuilder.ToString();
		}

		
		public static string GetCurrentVersionName()
		{
			Dictionary<string, NdpInfo> dictionary = ReadFromRegistry();
			if (!dictionary.Any())
			{
				return null;
			}
			NdpInfo value;
			if (dictionary.TryGetValue("v4 Full", out value))
			{
				return value.DisplayVersion;
			}
			if (dictionary.TryGetValue("v4", out value))
			{
				return value.DisplayVersion;
			}
			return dictionary.Last().Value.DisplayVersion;
		}

		public static Task<Dictionary<string, NdpInfo>> ReadFromRegistryAsync()
		{
			return Task.Run( () => ReadFromRegistry());
		}

		public static Dictionary<string, NdpInfo> ReadFromRegistry()
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
			{
				using (RegistryKey ndpKey = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\"))
				{
					return _003CReadFromRegistry_003Eg__ReadCore_007C24_0(ndpKey);
				}
			}
		}

		[CompilerGenerated]
		internal static Dictionary<string, NdpInfo> _003CReadFromRegistry_003Eg__ReadCore_007C24_0(RegistryKey ndpKey)
		{
			Dictionary<string, NdpInfo> dictionary = new Dictionary<string, NdpInfo>();
			if (ndpKey == null)
			{
				return dictionary;
			}
			string[] subKeyNames = ndpKey.GetSubKeyNames();
			foreach (string text in subKeyNames)
			{
				if (!text.StartsWith("v", StringComparison.InvariantCulture) || !(text != "v4.0"))
				{
					continue;
				}
				RegistryKey registryKey = ndpKey.OpenSubKey(text);
				if (registryKey == null)
				{
					continue;
				}
				string text2 = (string)registryKey.GetValue("Version", "");
				int sp = (int)registryKey.GetValue("SP", 0);
				string text3 = registryKey.GetValue("Install", "").ToString();
				int release = (int)registryKey.GetValue("Release", 0);
				if (!string.IsNullOrEmpty(text3) && text3 == "1")
				{
					dictionary[text] = new NdpInfo(text, null, text2, release, sp);
				}
				if (!string.IsNullOrEmpty(text2))
				{
					continue;
				}
				string[] subKeyNames2 = registryKey.GetSubKeyNames();
				foreach (string text4 in subKeyNames2)
				{
					RegistryKey registryKey2 = registryKey.OpenSubKey(text4);
					if (registryKey2 != null)
					{
						text2 = (string)registryKey2.GetValue("Version", "");
						if (!string.IsNullOrEmpty(text2))
						{
							sp = (int)registryKey2.GetValue("SP", 0);
						}
						text3 = registryKey2.GetValue("Install", "").ToString();
						release = (int)registryKey2.GetValue("Release", 0);
						if (!string.IsNullOrEmpty(text3) && text3 == "1")
						{
							dictionary[text + " " + text4] = new NdpInfo(text, text4, text2, release, sp);
						}
					}
				}
			}
			return dictionary;
		}
	}
}
