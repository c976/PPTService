using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Myou
{
	
	
	[Obsolete("请业务层自行使用 IApiMonitorResponse 和 IApiMonitor 两个接口实现对 Web 请求的监控。")]
	[DataContract]
	public class MonitorItem
	{
		[DataMember(Name = "monitor_type")]
		public string Type { get; set; }

		[DataMember(Name = "http_method")]
		public string Method { get; set; }

		[DataMember(Name = "status_code")]
		public int Code { get; set; }

		[DataMember(Name = "service_address")]
		public string Url { get; set; }

		[DataMember(Name = "time_cost")]
		public long Cost { get; set; }

		[DataMember(Name = "date_time")]
		public string Time { get; set; }

		[DataMember(Name = "error_message")]
		public string Message { get; set; }

		public MonitorItem()
		{
			Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
		}
	}
}
