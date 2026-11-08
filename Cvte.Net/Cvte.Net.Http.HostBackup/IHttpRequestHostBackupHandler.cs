using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.Http.HostBackup
{
	
	public interface IHttpRequestHostBackupHandler
	{
		Task<bool> TryGetNextBackupContext(HttpWebResponseWrapper response, HttpRequestHostBackupContext context);

		Task<bool> TryGetBackupContext(HttpRequestHostBackupContext context);
	}
}
