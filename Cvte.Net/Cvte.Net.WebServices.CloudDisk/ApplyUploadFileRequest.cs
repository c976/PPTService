using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[Request("/api/v1/upload/")]
	[DataContract]
	internal class ApplyUploadFileRequest : CloudDiskHttpRequest
	{
		[DataMember(Name = "parentUid")]
		public string DirectoryId { get; set; }

		[DataMember(Name = "items")]
		public IList<FileItem> Files { get; set; }
	}
}
