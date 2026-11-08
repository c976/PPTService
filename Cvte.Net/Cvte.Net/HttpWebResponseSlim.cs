using System.Net;
using System.Runtime.CompilerServices;

namespace Cvte.Net
{
	
	
	public class HttpWebResponseSlim
	{
		internal HttpStatusCode StatusCode { get; }

		internal CookieCollection Cookies { get; }

		internal WebHeaderCollection Headers { get; }

		internal long ContentLength { get; }

		internal HttpWebResponseSlim(HttpWebResponse response)
		{
			StatusCode = response.StatusCode;
			Cookies = response.Cookies;
			ContentLength = response.ContentLength;
			Headers = response.Headers;
		}
	}
}
