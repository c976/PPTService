using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	[DataContract]
	internal class UploadFileLycheeResponse : JsonResponse
	{
		public override bool IsSuccessBusiness => !string.IsNullOrEmpty(Url);

		[DataMember(Name = "url")]
		public new string Url { get; set; }
	}
}
