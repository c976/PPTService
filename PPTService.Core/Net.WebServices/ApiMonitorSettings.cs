using System;
using System.Runtime.CompilerServices;
using PPTService.Core.Net.WebServices.Monitors;

namespace PPTService.Core.Net.WebServices
{
	
	
	public class ApiMonitorSettings
	{
		
		
		public IApiMonitor ApiMonitor
		{
			
			get;
			
			set;
		}

		public bool Enable { get; set; } = true;

		[Obsolete("接口监控由业务层具体实现，并在此 Setting 设置 IApiMonitor 接口完成。此属性已无意义", true)]
		public string AppKey { get; set; } = "b6d5b2585940873e9b95a529b861e12e38d411a4";

		[Obsolete("接口监控由业务层具体实现，并在此 Setting 设置 IApiMonitor 接口完成。此属性已无意义", true)]
		public bool IsMonitorNormalstate { get; set; } = true;

		[Obsolete("接口监控由业务层具体实现，并在此 Setting 设置 IApiMonitor 接口完成。此属性已无意义", true)]
		public Func<string> DynamicCustomMessage { get; set; }
	}
}
