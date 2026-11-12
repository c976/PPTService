using System.Runtime.CompilerServices;
using PPTService.Core.Net.Http.HostBackup;

namespace PPTService.Core.Net.WebServices
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
