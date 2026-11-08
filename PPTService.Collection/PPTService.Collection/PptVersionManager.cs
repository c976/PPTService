using System.IO;
using System.Text.RegularExpressions;

namespace PPTService.Collection
{
	public static class PptVersionManager
	{
		private static string _version;

		private static string _softwareType;

		private static readonly object Locker = new object();

		public static bool IsWps => _softwareType == "wps";

		public static string Version
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_version))
				{
					return _version;
				}
				return OfficeVersion.Unknown.ToString();
			}
		}

		public static string SoftWareType => _softwareType;

		public static bool IsOffice2016OrLater { get; private set; }

		public static void UpdatePptApplicationInfo(string path, string versionInfo)
		{
			lock (Locker)
			{
				IsOffice2016OrLater = false;
				string path2 = Path.GetFullPath(path).ToLower();
				string version;
				if (TryGetOfficeVersion(path2, versionInfo, out version))
				{
					_softwareType = "office";
					_version = version;
				}
				else if (TryGetWpsVersion(path2, out version))
				{
					_softwareType = "wps";
					_version = version;
				}
				else
				{
					_version = null;
					_softwareType = null;
				}
			}
		}

		private static bool TryGetOfficeVersion(string path, string versionInfo, out string version)
		{
			version = null;
			if (!path.Contains("powerpoint") && !path.Contains("microsoft"))
			{
				return false;
			}
			switch (versionInfo)
			{
			case "12.0":
				version = OfficeVersion.Office2007.ToString();
				break;
			case "14.0":
				version = OfficeVersion.Office2010.ToString();
				break;
			case "15.0":
				version = OfficeVersion.Office2013.ToString();
				break;
			case "16.0":
				version = OfficeVersion.Office2016.ToString();
				IsOffice2016OrLater = true;
				break;
			case "17.0":
				version = OfficeVersion.Office2019.ToString();
				IsOffice2016OrLater = true;
				break;
			}
			return true;
		}

		private static bool TryGetWpsVersion(string path, out string version)
		{
			version = null;
			if (!path.Contains("kingsoft") && !path.Contains("wps") && !path.Contains("wpp"))
			{
				return false;
			}
			Match match = Regex.Match(path, "(\\d+\\.){3}\\d+");
			version = match.Value;
			return true;
		}
	}
}
