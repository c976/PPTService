using System.Runtime.Serialization;
using Cvte.Pipe.Http.Response;

namespace Cvte.WebService.Feedback
{
	[DataContract]
	internal class FeedbackResponse<TBody> : JsonResponse where TBody : ResponseBody
	{
		[DataMember(Name = "msg")]
		public string Message { get; set; }

		[DataMember(Name = "info")]
		public string Info { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

		[DataMember(Name = "data")]
		public TBody Data { get; set; }

		public override bool Succeed
		{
			get
			{
				if (Status != null)
				{
					return int.Parse(Status) == 200;
				}
				return false;
			}
		}
	}
}
