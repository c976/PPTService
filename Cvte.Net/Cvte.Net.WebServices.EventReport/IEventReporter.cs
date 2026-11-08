using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.EventReport
{
	
	public interface IEventReporter
	{
		void ReportEvent(string eventId);

		
		void ReportEvent<T>( string eventId, T eventValue) where T : struct;

		void ReportEvent(string eventId, string eventValue);

		void ReportEvent(string eventId, IDictionary<string, object> eventParams);

		void ReportEventImmediately(string eventId);

		
		void ReportEventImmediately<T>( string eventId, T eventValue) where T : struct;

		void ReportEventImmediately(string eventId, string eventValue);

		void ReportEventImmediately(string eventId, IDictionary<string, object> eventParams);
	}
}
