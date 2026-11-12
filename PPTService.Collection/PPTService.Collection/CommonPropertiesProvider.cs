using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using PPTService.Core.Net.WebServices.EventReport;

namespace PPTService.Collection
{
	internal class CommonPropertiesProvider : IEventCommonPropertiesProvider
	{
		private readonly IDictionary<string, object> _commonProperties;

		public static string GetMac
		{
			get
			{
				foreach (ManagementObject instance in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
				{
					if ((bool)instance["IPEnabled"])
					{
						return instance["MacAddress"].ToString().ToUpper();
					}
					instance.Dispose();
				}
				return null;
			}
		}

		public CommonPropertiesProvider(string appName, string appVersion)
		{
			string md5Str = GetMd5Str(GetMac + appName);
			_commonProperties = new Dictionary<string, object>(StringComparer.Ordinal)
			{
				{ "$tid", md5Str },
				{ "AppVersion", appVersion },
				{
					"OfficeOrWps",
					PptVersionManager.SoftWareType
				},
				{
					"OfficeVersion",
					PptVersionManager.Version
				}
			};
		}

		public IDictionary<string, object> BuildCommonProperties()
		{
			return _commonProperties;
		}

		public IDictionary<string, object> FilterCommonProperties(IDictionary<string, object> commonProperties)
		{
			commonProperties["OfficeOrWps"] = PptVersionManager.SoftWareType;
			commonProperties["OfficeVersion"] = PptVersionManager.Version;
			return commonProperties;
		}

		public static string GetMd5Str(string str)
		{
			byte[] bytes = new ASCIIEncoding().GetBytes(str);
			byte[] array = new MD5CryptoServiceProvider().ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}
	}
}
