using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTService.Core.Net.WebServices.EventReport.Models;
using PPTService.Core.Net.WebServices.EventReport.Utils;
using PPTService.Core.Net.WebServices.Myou;

namespace PPTService.Core.Net.WebServices.EventReport
{
	
	
	internal class PostEventRequestBuilder
	{
		public string AppId { get; }

		public string SessionId { get; }

		public bool IncludeDefaultProperties { get; set; }

		public PostEventRequestBuilder(string appId, string sessionId)
		{
			AppId = appId;
			SessionId = sessionId;
		}

		public PostEventRequest Build(IEnumerable<Event> events, IDictionary<string, object> commonProperties)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
			if (IncludeDefaultProperties)
			{
				IReadOnlyDictionary<string, object> hardwareProperties = PostEventHelper.HardwareProperties;
				dictionary.AddRange(hardwareProperties);
			}
			dictionary.AddRange(commonProperties);
			PostEventRequest postEventRequest = new PostEventRequest();
			postEventRequest.CommonProperties = dictionary;
			postEventRequest.EventProperties = GetEventProperties(events);
			postEventRequest.Init(AppId, SessionId);
			return postEventRequest;
		}

		private static IList<IDictionary<string, object>> GetEventProperties(IEnumerable<Event> events)
		{
			List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
			foreach (Event @event in events)
			{
				Dictionary<string, object> eventParams = new Dictionary<string, object>(StringComparer.Ordinal)
				{
					{ "$code", @event.Key },
					{ "$ts", @event.Timestamp }
				};
				if (@event.Params != null)
				{
					foreach (KeyValuePair<string, object> item in @event.Params.Where( (KeyValuePair<string, object> param) => !eventParams.ContainsKey(param.Key)))
					{
						eventParams.Add(item.Key, item.Value);
					}
				}
				list.Add(eventParams);
			}
			return list;
		}
	}
}
