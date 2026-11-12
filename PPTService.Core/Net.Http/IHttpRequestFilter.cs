using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.Http
{
	
	public interface IHttpRequestFilter
	{
		bool Match(HttpRequest request);

		void Handle(HttpRequest request);
	}
}
