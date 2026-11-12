using System;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.Http
{
	
	
	public class RequestAttribute : Attribute
	{
		public virtual string Api { get; }

		public RequestMethods RequestMethod { get; protected set; }

		public RequestAttribute(string api)
			: this(api, RequestMethods.Default)
		{
		}

		public RequestAttribute(string api, RequestMethods requestMethod)
		{
			Api = api;
			RequestMethod = requestMethod;
		}

		protected RequestAttribute()
		{
		}
	}
}
