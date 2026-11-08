using System;
using System.Runtime.CompilerServices;

namespace Cvte.Net.Http
{
	
	
	public class BasicHttpResponseFilter : BasicHttpResponseFilter<HttpResponse>
	{
		public BasicHttpResponseFilter(Action<HttpRequest, HttpResponse> action)
			: base(action)
		{
		}
	}
	
	
	public class BasicHttpResponseFilter< SA_T0> : IHttpResponseFilter where SA_T0 : HttpResponse
	{
		private readonly Action<HttpRequest, SA_T0> _action;

		public BasicHttpResponseFilter(Action<HttpRequest, SA_T0> action)
		{
			_action = action;
		}

		public bool Match(HttpResponse response)
		{
			return response is SA_T0;
		}

		public void Handle(HttpRequest request, HttpResponse response)
		{
			SA_T0 val = response as SA_T0;
			if (val != null)
			{
				_action(request, val);
			}
		}
	}
}
