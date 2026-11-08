using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[DataContract]
	public class GetFilesResponse : CloudDiskHttpResponse
	{
		[DataMember(Name = "totalElements")]
		public int Total { get; internal set; }

		[DataMember(Name = "content")]
		public List<FileItem> Files { get; internal set; }
	}
}
