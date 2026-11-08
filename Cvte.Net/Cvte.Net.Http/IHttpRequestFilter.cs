using System.Runtime.CompilerServices;

namespace Cvte.Net.Http
{
	
	public interface IHttpRequestFilter
	{
		bool Match(HttpRequest request);

		void Handle(HttpRequest request);
	}
}
