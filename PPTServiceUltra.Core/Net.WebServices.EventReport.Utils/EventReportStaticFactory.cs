using System.Runtime.CompilerServices;
using PPTServiceUltra.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Core.Net.WebServices.EventReport.Utils
{
	
	
	public static class EventReportStaticFactory
	{
		[CanBeNull]
		public static IEventReportStaticPropertyProvider StaticPropertyProvider { get; set; }
	}
}
