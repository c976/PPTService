using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Monitors
{
	
	public interface IApiMonitor
	{
		Task Report(HttpRequest request, HttpResponse response, long elapsedTime);
	}
}
