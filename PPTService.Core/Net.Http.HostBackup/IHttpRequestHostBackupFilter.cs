using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.Http.HostBackup
{
	
	public interface IHttpRequestHostBackupFilter
	{
		IHttpRequestHostBackupHandler BuildHandler(HttpRequestHostBackupContext context);
	}
}
