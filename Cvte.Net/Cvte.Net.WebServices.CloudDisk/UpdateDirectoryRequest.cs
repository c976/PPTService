using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[Request("", RequestMethods.Put)]
	[DataContract]
	internal class UpdateDirectoryRequest : CloudDiskHttpRequest
	{
		[DataMember(Name = "uid")]
		public string DirectoryId { get; internal set; }

		[DataMember(Name = "name")]
		public string Name { get; internal set; }

		public UpdateDirectoryRequest(string directoryId)
		{
			Url = "/api/v1/directories/" + directoryId;
			DirectoryId = directoryId;
		}
	}
}
