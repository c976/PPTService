using System.Runtime.CompilerServices;

namespace Cvte.Net.Http.HostBackup
{
	
	public interface IHttpRequestHostBackupFilter
	{
		IHttpRequestHostBackupHandler BuildHandler(HttpRequestHostBackupContext context);
	}
}
