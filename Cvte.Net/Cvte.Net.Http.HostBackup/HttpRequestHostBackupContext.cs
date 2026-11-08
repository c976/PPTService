using System;
using System.Runtime.CompilerServices;

namespace Cvte.Net.Http.HostBackup
{
	
	
	public class HttpRequestHostBackupContext
	{
		public string MethodName { get; }

		public string OriginUrl { get; }

		public string Url { get; set; }

		public bool IsOriginContext => string.Equals(OriginUrl, Url, StringComparison.OrdinalIgnoreCase);

		public string Host { get; set; } = "";

		
		
		public string ExtensionStatus
		{
			
			get;
			
			set;
		}

		public HttpRequestHostBackupContext(string url, string methodName)
		{
			Url = url;
			OriginUrl = url;
			MethodName = methodName;
		}
	}
}
