using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.WebServices.EventReport.Models
{
	
	
	internal class Event
	{
		internal string Key { get; private set; }

		internal long Timestamp { get; private set; }

		
		
		internal IDictionary<string, object> Params
		{
			
			get;
			
			private set;
		}

		public Event(string eventKey)
		{
			Init(eventKey, null);
		}

		public Event(string eventKey,  object eventValue)
		{
			if (eventValue == null)
			{
				Init(eventKey, null);
				return;
			}
			Init(eventKey, new EventParamDictionary(new ValueTuple<string, object>("value", eventValue)));
		}

		public Event(string eventKey,  IDictionary<string, object> eventParams)
		{
			Init(eventKey, eventParams);
		}

		private void Init(string eventKey,  IDictionary<string, object> eventParams)
		{
			Key = eventKey;
			Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
			Params = eventParams;
		}
	}
}
