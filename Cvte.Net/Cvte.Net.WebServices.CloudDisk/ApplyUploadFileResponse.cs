using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[DataContract]
	internal class ApplyUploadFileResponse : CloudDiskHttpResponse
	{
		[DataMember(Name = "uploadUrl")]
		public string UploadUrl { get; set; }

		[DataMember(Name = "params")]
		public IDictionary<string, string> UploadParams { get; set; }

		[DataMember(Name = "items")]
		public IList<FileItem> Files { get; set; }

		[DataMember(Name = "fileFieldName")]
		public string FileKey { get; set; }

		[DataMember(Name = "keyName")]
		public string KeyName { get; set; }
	}
}
