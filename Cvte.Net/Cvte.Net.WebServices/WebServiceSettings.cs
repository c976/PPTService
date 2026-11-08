using System.Runtime.CompilerServices;
using Cvte.Net.Http.HostBackup;

namespace Cvte.Net.WebServices
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
