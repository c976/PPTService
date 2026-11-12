using System.Runtime.CompilerServices;
using PPTService.Core.Net.Http;

namespace PPTService.Core.Net.WebServices
{
	
	public interface IWebService
	{
		void AddRequestFilter(IHttpRequestFilter httpRequestFilter);

		void AddResponseFilter(IHttpResponseFilter httpResponseFilter);
	}
}
