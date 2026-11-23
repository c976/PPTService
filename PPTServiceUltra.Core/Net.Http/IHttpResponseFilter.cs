using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Core.Net.Http
{
	
	public interface IHttpResponseFilter
	{
		bool Match(HttpResponse response);

		void Handle(HttpRequest request, HttpResponse response);
	}
}
