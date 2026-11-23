using System.Runtime.Serialization;
using PPTServiceUltra.Core.Pipe.Http.Response;

namespace PPTServiceUltra.Core.WebService.Lychee
{
	[DataContract]
	internal class UploadFileLycheeResponse : JsonResponse
	{
		[DataMember(Name = "ret")]
		public string Code { get; set; }

		public override bool Succeed => Code == "ok";
	}
}
