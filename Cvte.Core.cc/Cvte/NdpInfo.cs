using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cvte.Diagnostics.Annotations;
using Microsoft.Win32;

namespace Cvte
{
	public sealed class NdpInfo
	{
		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Dictionary<string, NdpInfo>> _003C_003E9__23_0;

			internal Dictionary<string, NdpInfo> _003CReadFromRegistryAsync_003Eb__23_0()
			{
				return ReadFromRegistry();
			}
		}

		private static readonly Dictionary<int, string> ReleaseToNameDictionary;

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
				Version version2 = default(Version);
				while (true)
				{
					if (2 != 0)
					{
						if (false || ReleaseToNameDictionary.TryGetValue(Release, out value))
						{
							break;
						}
						if (0 == 0)
						{
							Version version = new Version(Version);
							if (0 == 0)
							{
								version2 = version;
							}
						}
					}
					if (7 != 0)
					{
						return $"{version2.Major}.{version2.Minor}";
					}
				}
				return value;
			}
		}

		public string DisplayReleaseVersionName
		{
			get
			{
				while (true)
				{
					if (-1 != 0)
					{
						if (false || string.IsNullOrWhiteSpace(ReleaseVersionName))
						{
							goto IL_001e;
						}
						bool flag = ReleaseVersionName == "Full";
						if (0 == 0)
						{
							if (flag)
							{
								goto IL_001e;
							}
							flag = ReleaseVersionName == "Client";
						}
						if (!flag)
						{
							break;
						}
					}
					return "Client Profile";
					IL_001e:
					if (8 != 0)
					{
						return "";
					}
				}
				return ReleaseVersionName;
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
			if (dictionary.Any())
			{
				NdpInfo value;
				bool flag = dictionary.TryGetValue("v4 Full", out value);
				if (8 != 0 && 0 == 0)
				{
					if (flag)
					{
						return value.DisplayVersion;
					}
					flag = dictionary.TryGetValue("v4", out value);
				}
				if (flag)
				{
					return value.DisplayVersion;
				}
				if (0 == 0)
				{
					return dictionary.Last().Value.DisplayVersion;
				}
			}
			return null;
		}

		public static Task<Dictionary<string, NdpInfo>> ReadFromRegistryAsync()
		{
			Func<Dictionary<string, NdpInfo>> func;
			if (true)
			{
				func = _003C_003Ec._003C_003E9__23_0;
				if (func != null)
				{
					goto IL_0025;
				}
			}
			func = (_003C_003Ec._003C_003E9__23_0 = () => ReadFromRegistry());
			goto IL_0025;
			IL_0025:
			return Task.Run(func);
		}

		[NotNull]
		public unsafe static Dictionary<string, NdpInfo> ReadFromRegistry()
		{
			void* ptr = stackalloc byte[16];
			Dictionary<string, NdpInfo> dictionary = new Dictionary<string, NdpInfo>();
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\"))
			{
				if (registryKey != null)
				{
					string[] subKeyNames = registryKey.GetSubKeyNames();
					*(int*)ptr = 0;
					RegistryKey registryKey2 = default(RegistryKey);
					string text2 = default(string);
					string text3 = default(string);
					string[] subKeyNames2 = default(string[]);
					RegistryKey registryKey3 = default(RegistryKey);
					string text4 = default(string);
					for (; *(int*)ptr < subKeyNames.Length; (*(int*)ptr)++)
					{
						string text = subKeyNames[*(int*)ptr];
						if (2 == 0)
						{
							goto IL_01bc;
						}
						if (!text.StartsWith("v") || !(text != "v4.0"))
						{
							continue;
						}
						if (5 == 0)
						{
							goto IL_01fd;
						}
						registryKey2 = registryKey.OpenSubKey(text);
						if (registryKey2 == null)
						{
							continue;
						}
						text2 = (string)registryKey2.GetValue("Version", "");
						((int*)ptr)[1] = (int)registryKey2.GetValue("SP", 0);
						text3 = registryKey2.GetValue("Install", "").ToString();
						((int*)ptr)[2] = (int)registryKey2.GetValue("Release", 0);
						if (!(text3 == "") && text3 == "1")
						{
							dictionary[text] = new NdpInfo(text, null, text2, ((int*)ptr)[2], ((int*)ptr)[1]);
						}
						if (text2 != "")
						{
							continue;
						}
						subKeyNames2 = registryKey2.GetSubKeyNames();
						((int*)ptr)[3] = 0;
						goto IL_023e;
						IL_01bc:
						text3 = registryKey3.GetValue("Install", "").ToString();
						((int*)ptr)[2] = (int)registryKey3.GetValue("Release", 0);
						if (!(text3 == ""))
						{
							goto IL_01fd;
						}
						goto IL_0232;
						IL_0232:
						((int*)ptr)[3]++;
						goto IL_023e;
						IL_01fd:
						if (text3 == "1")
						{
							dictionary[text + " " + text4] = new NdpInfo(text, text4, text2, ((int*)ptr)[2], ((int*)ptr)[1]);
						}
						goto IL_0232;
						IL_023e:
						if (((int*)ptr)[3] >= subKeyNames2.Length)
						{
							continue;
						}
						text4 = subKeyNames2[((int*)ptr)[3]];
						registryKey3 = registryKey2.OpenSubKey(text4);
						if (registryKey3 != null)
						{
							text2 = (string)registryKey3.GetValue("Version", "");
							if (text2 != "")
							{
								((int*)ptr)[1] = (int)registryKey3.GetValue("SP", 0);
							}
							goto IL_01bc;
						}
						goto IL_0232;
					}
					return dictionary;
				}
				return dictionary;
			}
		}

		static NdpInfo()
		{
			if (true)
			{
				ReleaseToNameDictionary = new Dictionary<int, string>
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
			}
		}
	}
}
