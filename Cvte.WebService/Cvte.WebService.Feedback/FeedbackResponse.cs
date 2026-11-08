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
				int num;
				while (true)
				{
					if (Status == null && 3 != 0)
					{
						if (false)
						{
							continue;
						}
						num = 0;
						if (num == 0)
						{
							break;
						}
					}
					else
					{
						num = SA_Proxy._0098___0086(Status);
					}
					if (0 == 0)
					{
						num = ((num == 200) ? 1 : 0);
					}
					return (byte)num != 0;
				}
				return (byte)num != 0;
			}
		}
	}
}
