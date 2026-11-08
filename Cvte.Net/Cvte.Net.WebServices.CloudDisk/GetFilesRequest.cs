using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[Request("/api/v1/resources")]
	[DataContract]
	internal class GetFilesRequest : CloudDiskHttpRequest
	{
		[DataMember(Name = "parentUid")]
		public string DirectoryId { get; set; }

		[DataMember(Name = "page")]
		public int PageIndex { get; set; }

		[DataMember(Name = "size")]
		public int PageSize { get; set; }
	}
}
