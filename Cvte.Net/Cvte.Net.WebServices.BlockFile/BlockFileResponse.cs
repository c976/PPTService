using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	[DataContract]
	public class BlockFileResponse : JsonResponse
	{
		[DataMember(Name = "ctx")]
		public string Ctx { get; set; }

		[DataMember(Name = "checksum")]
		public string CheckSum { get; set; }

		[DataMember(Name = "crc32")]
		public long Crc32 { get; set; }

		[DataMember(Name = "offset")]
		public long Offset { get; set; }

		[DataMember(Name = "expired_at")]
		public long ExpiredAt { get; set; }
	}
}
