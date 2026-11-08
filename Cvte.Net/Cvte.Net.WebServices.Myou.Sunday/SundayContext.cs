using System;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Myou.Sunday
{
	
	
	public class SundayContext
	{
		public string SundayAppId { get; }

		public string SundayAppSecret { get; }

		public string AppName { get; }

		public string AppVersion { get; }

		public string AppSource { get; set; }

		public string UserId { get; set; }

		public string DeviceName { get; set; }

		public string DeviceId { get; set; }

		public string ComponentName { get; set; }

		public string ComponentVersion { get; set; }

		public SundayContext(string sundayAppId, string sundayAppSecret, string appName, string appVersion)
		{
			if (string.IsNullOrWhiteSpace(sundayAppId))
			{
				throw new ArgumentNullException("sundayAppId");
			}
			if (string.IsNullOrWhiteSpace(sundayAppSecret))
			{
				throw new ArgumentNullException("sundayAppSecret");
			}
			if (string.IsNullOrWhiteSpace(appName))
			{
				throw new ArgumentNullException("appName");
			}
			SundayAppId = sundayAppId;
			SundayAppSecret = sundayAppSecret;
			AppName = appName;
			AppVersion = appVersion;
		}
	}
}
