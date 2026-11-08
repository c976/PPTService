using System.Runtime.CompilerServices;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Net.WebServices.EventReport.Utils
{
	
	
	public static class EventReportStaticFactory
	{
		[CanBeNull]
		public static IEventReportStaticPropertyProvider StaticPropertyProvider { get; set; }
	}
}
