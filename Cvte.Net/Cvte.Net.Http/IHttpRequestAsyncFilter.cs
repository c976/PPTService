using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.Http
{
	
	public interface IHttpRequestAsyncFilter
	{
		bool Match(HttpRequest request);

		Task HandleAsync(HttpRequest request);
	}
}
