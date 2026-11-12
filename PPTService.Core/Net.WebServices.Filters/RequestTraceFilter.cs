using System;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTService.Core.Net.Http;

namespace PPTService.Core.Net.WebServices.Filters
{
	
	
	public class RequestTraceFilter : IHttpRequestFilter
	{
		public const string ApmTraceIdHeader = "X-APM-TraceId";

		public bool Match(HttpRequest request)
		{
			return true;
		}

		public void Handle(HttpRequest request)
		{
			if (!request.HeaderCollection.AllKeys.Contains("X-APM-TraceId", StringComparer.Ordinal))
			{
				string value = null;
				IHttpRequestTraceable httpRequestTraceable = request as IHttpRequestTraceable;
				if (httpRequestTraceable != null)
				{
					value = httpRequestTraceable.TraceId;
				}
				if (string.IsNullOrEmpty(value))
				{
					value = Guid.NewGuid().ToString("N");
				}
				request.HeaderCollection.Add("X-APM-TraceId", value);
			}
		}
	}
}
