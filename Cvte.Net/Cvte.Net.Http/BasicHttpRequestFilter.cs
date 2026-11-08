using System;
using System.Runtime.CompilerServices;

namespace Cvte.Net.Http
{
	
	
	public class BasicHttpRequestFilter : BasicHttpRequestFilter<HttpRequest>
	{
		public BasicHttpRequestFilter(Action<HttpRequest> action)
			: base(action)
		{
		}
	}
	
	
	public class BasicHttpRequestFilter< SA_T0> : IHttpRequestFilter where SA_T0 : HttpRequest
	{
		private readonly Action<SA_T0> _action;

		public BasicHttpRequestFilter(Action<SA_T0> action)
		{
			_action = action;
		}

		public bool Match(HttpRequest request)
		{
			return request is SA_T0;
		}

		public void Handle(HttpRequest request)
		{
			SA_T0 val = request as SA_T0;
			if (val != null)
			{
				_action(val);
			}
		}
	}
}
