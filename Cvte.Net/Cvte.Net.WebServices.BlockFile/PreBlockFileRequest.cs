using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	[Request("/api/v1/upload/getSinglePartUploadToken")]
	[DataContract]
	public class PreBlockFileRequest : JsonRequest
	{
		[DataMember(Name = "part")]
		public long Part { get; set; }

		[DataMember(Name = "uploadId")]
		public string UploadId { get; set; }
	}
}
