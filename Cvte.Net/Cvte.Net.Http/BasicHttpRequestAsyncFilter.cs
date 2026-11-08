using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.Http
{
	
	
	public class BasicHttpRequestAsyncFilter : BasicHttpRequestAsyncFilter<HttpRequest>
	{
		public BasicHttpRequestAsyncFilter(Func<HttpRequest, Task> asyncAction)
			: base(asyncAction)
		{
		}
	}
	
	
	public class BasicHttpRequestAsyncFilter< SA_T0> : IHttpRequestAsyncFilter where SA_T0 : HttpRequest
	{
		private readonly Func<SA_T0, Task> _asyncAction;

		public BasicHttpRequestAsyncFilter(Func<SA_T0, Task> asyncAction)
		{
			_asyncAction = asyncAction;
		}

		public bool Match(HttpRequest request)
		{
			return request is SA_T0;
		}

		public async Task HandleAsync(HttpRequest request)
		{
			SA_T0 val = request as SA_T0;
			if (val != null)
			{
				await _asyncAction(val);
			}
		}
	}
}
