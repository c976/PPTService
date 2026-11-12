using System.Runtime.Serialization;
using PPTService.Core.Pipe.Http.Response;

namespace PPTService.Core.WebService.Lychee
{
	[DataContract]
	internal class UploadFileLycheeResponse : JsonResponse
	{
		[DataMember(Name = "ret")]
		public string Code { get; set; }

		public override bool Succeed => Code == "ok";
	}
}
