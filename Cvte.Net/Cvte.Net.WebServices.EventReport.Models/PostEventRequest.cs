using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.EventReport.Models
{
	
	
	[DataContract]
	internal class PostEventRequest : JsonRequest
	{
		public override string ContentType => "text/json;charset=utf-8";

		[DataMember(Name = "$cp")]
		public IDictionary<string, object> CommonProperties { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);

		[DataMember(Name = "$sp")]
		public IList<IDictionary<string, object>> EventProperties { get; set; } = new List<IDictionary<string, object>>();

		[DataMember(Name = "$tp")]
		public IDictionary<string, object> TriggerProperties { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);

		public PostEventRequest()
		{
			Url = "https://myou.cvte.com/friday/agent/api/app/v2/report";
		}

		public void Init(string appId, string sessionId)
		{
			long timespan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
			Init(appId, sessionId, timespan);
		}

		internal void Init(string appId, string sessionId, long timespan)
		{
			HeaderCollection.Set("X-Friday-Appid", appId);
			HeaderCollection.Set("X-Friday-Time", timespan.ToString(CultureInfo.InvariantCulture));
			CookieCollection.Add(new Cookie("sessionId", sessionId));
		}
	}
}
