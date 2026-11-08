using System.Runtime.CompilerServices;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices
{
	
	public interface IWebService
	{
		void AddRequestFilter(IHttpRequestFilter httpRequestFilter);

		void AddResponseFilter(IHttpResponseFilter httpResponseFilter);
	}
}
