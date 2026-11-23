using System.Runtime.CompilerServices;
using PPTServiceUltra.Core.Net.Http.HostBackup;

namespace PPTServiceUltra.Core.Net.WebServices
{
	
	
	public class WebServiceSettings
	{
		public ApiMonitorSettings ApiMonitorSetting { get; set; } = new ApiMonitorSettings();

		
		
		public IHttpRequestHostBackupFilter HostBackupFilter
		{
			
			get;
			
			set;
		}
	}
}
