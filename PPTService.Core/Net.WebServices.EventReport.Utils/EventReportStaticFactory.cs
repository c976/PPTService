using System.Runtime.CompilerServices;
using PPTService.Core.Diagnostics.Annotations;

namespace PPTService.Core.Net.WebServices.EventReport.Utils
{
	
	
	public static class EventReportStaticFactory
	{
		[CanBeNull]
		public static IEventReportStaticPropertyProvider StaticPropertyProvider { get; set; }
	}
}
