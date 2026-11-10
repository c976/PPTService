using System;
using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Feedback
{
	[DataContract]
	internal class FeedbackReqeustBase : JsonRequest
	{
		public string Timestamp { get; set; }

		public string AuthId { get; set; }

		public FeedbackReqeustBase()
		{
			Timestamp = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString("0000000000000");
		}

		public override string GetRequestUrl()
		{
			return base.GetRequestUrl() + "?authId=" + AuthId + "&timestamp=" + Timestamp;
		}
	}
}
