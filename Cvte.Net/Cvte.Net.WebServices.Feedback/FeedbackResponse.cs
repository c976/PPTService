using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	[DataContract]
	internal class FeedbackResponse< SA_T0> : JsonResponse where SA_T0 : ResponseBody
	{
		[DataMember(Name = "msg")]
		public string Message { get; set; }

		[DataMember(Name = "info")]
		public string Info { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

		[DataMember(Name = "data")]
		public SA_T0 Data { get; set; }

		public override bool IsSuccessBusiness
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
