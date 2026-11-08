using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cvte.Net.WebServices.Myou;
using lindexi.src;

namespace Cvte.Net.WebServices.EventReport.Utils
{
	
	
	internal static class PostEventHelper
	{
		private static readonly Lazy<IReadOnlyDictionary<string, object>> _hardwareProperties = new Lazy<IReadOnlyDictionary<string, object>>(() => new Dictionary<string, object>(StringComparer.Ordinal)
		{
			{
				"os",
				Environment.OSVersion.VersionString
			},
			{
				"cpu",
				EnvironmentUtility.CPU
			},
			{
				"ram",
				EnvironmentUtility.RAM
			},
			{
				"time_zone",
				EnvironmentUtility.TimeZone
			},
			{
				"clr",
				Environment.Version
			},
			{
				"ndp",
				EnvironmentUtility.NdpVersion
			},
			{
				"x64",
				Environment.Is64BitOperatingSystem.ToString(CultureInfo.InvariantCulture)
			},
			{
				"stable_mac_id",
				EnvironmentUtility.MacAddressId
			},
			{
				"mac_list",
				string.Join(";", MacAddress.GetActiveMacAddress(":"))
			}
		});

		public const int EventBatchSize = 10;

		public const string CacheFileExtension = ".edc";

		public static IReadOnlyDictionary<string, object> HardwareProperties => _hardwareProperties.Value;

		public static string SessionId
		{
			get
			{
				if (string.IsNullOrWhiteSpace(EventReportStaticFactory.StaticPropertyProvider?.SessionId))
				{
					return SessionIdProvider.SessionId;
				}
				return EventReportStaticFactory.StaticPropertyProvider.SessionId;
			}
		}

		public static async Task<IReadOnlyDictionary<string, object>> GetHardwarePropertiesAsync()
		{
			return await Task.Run( () => HardwareProperties).ConfigureAwait(false);
		}
	}
}
