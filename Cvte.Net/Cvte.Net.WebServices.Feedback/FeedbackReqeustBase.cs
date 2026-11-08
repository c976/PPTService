using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	[DataContract]
	internal class FeedbackReqeustBase : RestfulRequest
	{
		[RestfulMember(Name = "timestamp")]
		public string Timestamp { get; set; }

		[RestfulMember(Name = "authId")]
		public string AuthId { get; set; }

		public FeedbackReqeustBase()
		{
			Timestamp = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString("0000000000000");
		}
	}
}
