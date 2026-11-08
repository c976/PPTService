using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cvte.Net.WebServices.EventReport.Models;
using Cvte.Net.WebServices.EventReport.Utils;

namespace Cvte.Net.WebServices.EventReport
{
	
	
	public class EventReporter : IEventReporter
	{
		private InnerEventReporter InnerEventReporter { get; }

		public EventReporter(string fridayAppId, IEventCommonPropertiesProvider commonPropertiesProvider)
			: this(fridayAppId, commonPropertiesProvider, true)
		{
		}

		public EventReporter(string fridayAppId, IEventCommonPropertiesProvider commonPropertiesProvider, bool includeDefaultProperties)
		{
			InnerEventReporter = new InnerEventReporter(fridayAppId, commonPropertiesProvider, includeDefaultProperties);
			if (includeDefaultProperties)
			{
				PostEventHelper.GetHardwarePropertiesAsync();
			}
		}

		public virtual void ReportEvent(string eventId)
		{
			InnerEventReporter.ReportEvent(new Event(eventId));
		}

		
		public virtual void ReportEvent<T>( string eventId, T eventValue) where T : struct
		{
			InnerEventReporter.ReportEvent(new Event(eventId, eventValue));
		}

		public virtual void ReportEvent(string eventId, string eventValue)
		{
			InnerEventReporter.ReportEvent(new Event(eventId, eventValue));
		}

		public virtual void ReportEvent(string eventId, IDictionary<string, object> eventParams)
		{
			InnerEventReporter.ReportEvent(new Event(eventId, eventParams));
		}

		public virtual void ReportEventImmediately(string eventId)
		{
			InnerEventReporter.ReportEventImmediately(new Event(eventId));
		}

		
		public virtual void ReportEventImmediately<T>( string eventId, T eventValue) where T : struct
		{
			InnerEventReporter.ReportEventImmediately(new Event(eventId, eventValue));
		}

		public virtual void ReportEventImmediately(string eventId, string eventValue)
		{
			InnerEventReporter.ReportEventImmediately(new Event(eventId, eventValue));
		}

		public virtual void ReportEventImmediately(string eventId, IDictionary<string, object> eventParams)
		{
			InnerEventReporter.ReportEventImmediately(new Event(eventId, eventParams));
		}

		public async Task FlushAsync()
		{
			await InnerEventReporter.FlushAsync().ConfigureAwait(false);
		}

		public async Task CacheDataAsync()
		{
			await InnerEventReporter.CacheDataAsync().ConfigureAwait(false);
		}

		public async Task FlushCacheDataAsync()
		{
			await InnerEventReporter.FlushCacheDataAsync().ConfigureAwait(false);
		}
	}
}
