using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PPTService.Core.Net.Http;

namespace PPTService.Core.Net.WebServices.Lychee
{
	
	
	[DataContract]
	internal class UploadFileLycheeResponse : JsonResponse
	{
		public override bool IsSuccessBusiness => !string.IsNullOrEmpty(Url);

		[DataMember(Name = "url")]
		public new string Url { get; set; }
	}
}
