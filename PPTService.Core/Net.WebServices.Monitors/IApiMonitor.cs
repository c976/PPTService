using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTService.Core.Net.Http;

namespace PPTService.Core.Net.WebServices.Monitors
{
	
	public interface IApiMonitor
	{
		Task Report(HttpRequest request, HttpResponse response, long elapsedTime);
	}
}
