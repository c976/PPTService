using System.Runtime.Serialization;
using Cvte.Pipe.Http.Response;

namespace Cvte.WebService.Lychee
{
	[DataContract]
	internal class UploadFileLycheeResponse : JsonResponse
	{
		[DataMember(Name = "ret")]
		public string Code { get; set; }

		public override bool Succeed => SA_Proxy._008B___001D(Code, "ok");
	}
}
