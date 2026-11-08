using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.Http
{
	
	
	[DataContract]
	public class QiniuResponse : JsonResponse
	{
		[DataMember(Name = "ret")]
		public string Code { get; set; }

		public override bool IsSuccessBusiness => Code == "ok";
	}
}
