using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Core.Net.Http.HostBackup
{
	
	public interface IHttpRequestHostBackupFilter
	{
		IHttpRequestHostBackupHandler BuildHandler(HttpRequestHostBackupContext context);
	}
}
