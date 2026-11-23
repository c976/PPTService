using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTServiceUltra.Core.Net.Http;

namespace PPTServiceUltra.Core.Net.WebServices.Monitors
{
	
	public interface IApiMonitor
	{
		Task Report(HttpRequest request, HttpResponse response, long elapsedTime);
	}
}
