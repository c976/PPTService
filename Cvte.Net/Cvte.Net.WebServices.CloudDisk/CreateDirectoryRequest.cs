using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[Request("/api/v1/directories")]
	[DataContract]
	internal class CreateDirectoryRequest : CloudDiskHttpRequest
	{
		[DataMember(Name = "name")]
		public string Name { get; internal set; }

		[DataMember(Name = "parentUid")]
		public string ParentId { get; set; }
	}
}
