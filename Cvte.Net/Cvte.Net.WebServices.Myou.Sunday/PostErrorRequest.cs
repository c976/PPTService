using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Myou.Sunday
{
	
	
	[DataContract]
	internal class PostErrorRequest : JsonRequest
	{
		[DataMember(Name = "name")]
		public string ErrorKey { get; set; }

		[DataMember(Name = "message")]
		public string ErrorSubKey { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "code")]
		public string Code { get; set; } = "bug";

		[DataMember(Name = "stack")]
		public string Stack { get; set; }

		[DataMember(Name = "tid")]
		public string User { get; set; }

		[DataMember(Name = "sid")]
		public string SessionId { get; set; }

		[DataMember(Name = "extra")]
		public object Context { get; set; }

		[DataMember(Name = "appName")]
		public string AppName { get; set; }

		[DataMember(Name = "version")]
		public string AppVersion { get; set; }

		[DataMember(Name = "appSource")]
		public string AppSource { get; set; }

		[DataMember(Name = "appPeriod")]
		public string AppPeriod { get; set; }

		[DataMember(Name = "appStatus")]
		public string AppStatus { get; set; }

		[DataMember(Name = "cbbName")]
		public string ComponentName { get; set; }

		[DataMember(Name = "cbbVersion")]
		public string ComponentVersion { get; set; }

		[DataMember(Name = "sysName")]
		public string OSName { get; set; }

		[DataMember(Name = "sysVersion")]
		public string OSVersion { get; set; }

		[DataMember(Name = "sysThread")]
		public string Thread { get; set; }

		[DataMember(Name = "sysLang")]
		public string OSLang { get; set; }

		[DataMember(Name = "devId")]
		public string DeviceId { get; set; }

		[DataMember(Name = "devName")]
		public string DeviceName { get; set; }

		[DataMember(Name = "devRom")]
		public string Ram { get; set; }

		[DataMember(Name = "devCpu")]
		public string Cpu { get; set; }

		[DataMember(Name = "devGraph")]
		public string DeviceGraph { get; set; }

		[DataMember(Name = "devRes")]
		public string ScreenResolution { get; set; }

		[DataMember(Name = "devImei")]
		public string Imei { get; set; }

		[DataMember(Name = "devImsi")]
		public string Imsi { get; set; }

		[DataMember(Name = "devNet")]
		public string Net { get; set; }

		[DataMember(Name = "devMac")]
		public string Mac { get; set; }

		[DataMember(Name = "ts")]
		public long TimeSpan { get; set; }

		public void Init(string authAppId, string authAppSecret, string url)
		{
			Url = url;
			long num = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
			HeaderCollection.Add("x-auth-time", num.ToString());
			HeaderCollection.Add("x-auth-app", authAppId);
			string value = SecurityUtility.Encrypt32($"&time={num}&appSecret={authAppSecret}");
			if (!string.IsNullOrEmpty(value))
			{
				HeaderCollection.Add("x-auth-sign", value);
			}
			TimeSpan = num;
		}
	}
}
