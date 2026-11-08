using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Myou.Sunday
{
	internal class PostErrorResponse : JsonResponse
	{
		[DataMember(Name = "status")]
		public int Status { get; set; }
	}
}
