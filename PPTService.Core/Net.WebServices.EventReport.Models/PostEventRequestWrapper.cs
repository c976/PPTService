using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PPTService.Core.Net.WebServices.EventReport.Models
{
	
	
	[DataContract(Name = "event")]
	internal class PostEventRequestWrapper
	{
		[DataMember(Name = "appId")]
		public string AppId { get; set; }

		[DataMember(Name = "sessionId")]
		public string SessionId { get; set; }

		[DataMember(Name = "timespan")]
		public long TimeSpan { get; set; }

		[DataMember(Name = "request")]
		public PostEventRequest Request { get; set; }
	}
}
