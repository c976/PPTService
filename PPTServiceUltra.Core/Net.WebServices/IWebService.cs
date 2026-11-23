using System.Runtime.CompilerServices;
using PPTServiceUltra.Core.Net.Http;

namespace PPTServiceUltra.Core.Net.WebServices
{
	
	public interface IWebService
	{
		void AddRequestFilter(IHttpRequestFilter httpRequestFilter);

		void AddResponseFilter(IHttpResponseFilter httpResponseFilter);
	}
}
