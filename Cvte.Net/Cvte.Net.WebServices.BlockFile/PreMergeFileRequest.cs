using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	[Request("/api/v1/upload/getMergeFileToken")]
	[DataContract]
	public class PreMergeFileRequest : JsonRequest
	{
		[DataMember(Name = "uploadId")]
		public string UploadId { get; set; }
	}
}
