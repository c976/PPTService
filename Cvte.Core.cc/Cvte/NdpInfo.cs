using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cvte.Diagnostics.Annotations;
using Microsoft.Win32;

namespace Cvte
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
			{ 461310, "4.7.1" }
		};

		[NotNull]
		public string MainVersion { get; }

		[CanBeNull]
		public string ReleaseVersionName { get; }

		[NotNull]
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
				if (!string.IsNullOrWhiteSpace(ReleaseVersionName) && !(ReleaseVersionName == "Full"))
				{
					if (ReleaseVersionName == "Client")
					{
						return "Client Profile";
					}
					return ReleaseVersionName;
				}
				return "";
			}
		}

		private NdpInfo(string mainVersion, string releaseVersionName, string version, int release, int sp)
		{
			MainVersion = mainVersion;
			ReleaseVersionName = releaseVersionName;
			Version = version;
			Release = release;
			SP = sp;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder($".NET Framework {DisplayVersion}");
			if (SP > 0)
			{
				stringBuilder.Append($" SP{SP}");
			}
			string displayReleaseVersionName = DisplayReleaseVersionName;
			if (!string.IsNullOrWhiteSpace(displayReleaseVersionName))
			{
				stringBuilder.Append($" {displayReleaseVersionName}");
			}
			return stringBuilder.ToString();
		}

		[CanBeNull]
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
			return Task.Run(() => ReadFromRegistry());
		}

		[NotNull]
		public unsafe static Dictionary<string, NdpInfo> ReadFromRegistry()
		{
			void* ptr = stackalloc byte[16];
			Dictionary<string, NdpInfo> dictionary = new Dictionary<string, NdpInfo>();
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\"))
			{
				if (registryKey == null)
				{
					return dictionary;
				}
				string[] subKeyNames = registryKey.GetSubKeyNames();
				*(int*)ptr = 0;
				while (*(int*)ptr < subKeyNames.Length)
				{
					string text = subKeyNames[*(int*)ptr];
					if (text.StartsWith("v") && text != "v4.0")
					{
						RegistryKey registryKey2 = registryKey.OpenSubKey(text);
						if (registryKey2 != null)
						{
							string text2 = (string)registryKey2.GetValue("Version", "");
							((int*)ptr)[1] = (int)registryKey2.GetValue("SP", 0);
							string text3 = registryKey2.GetValue("Install", "").ToString();
							((int*)ptr)[2] = (int)registryKey2.GetValue("Release", 0);
							if (!(text3 == "") && text3 == "1")
							{
								dictionary[text] = new NdpInfo(text, null, text2, ((int*)ptr)[2], ((int*)ptr)[1]);
							}
							if (!(text2 != ""))
							{
								string[] subKeyNames2 = registryKey2.GetSubKeyNames();
								((int*)ptr)[3] = 0;
								while (((int*)ptr)[3] < subKeyNames2.Length)
								{
									string text4 = subKeyNames2[((int*)ptr)[3]];
									RegistryKey registryKey3 = registryKey2.OpenSubKey(text4);
									if (registryKey3 != null)
									{
										text2 = (string)registryKey3.GetValue("Version", "");
										if (text2 != "")
										{
											((int*)ptr)[1] = (int)registryKey3.GetValue("SP", 0);
										}
										text3 = registryKey3.GetValue("Install", "").ToString();
										((int*)ptr)[2] = (int)registryKey3.GetValue("Release", 0);
										if (!(text3 == "") && text3 == "1")
										{
											dictionary[text + " " + text4] = new NdpInfo(text, text4, text2, ((int*)ptr)[2], ((int*)ptr)[1]);
										}
									}
									((int*)ptr)[3]++;
								}
							}
						}
					}
					(*(int*)ptr)++;
				}
				return dictionary;
			}
		}
	}
}
