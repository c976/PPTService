using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[Request("", RequestMethods.Put)]
	internal class UpdateFileRequest : CloudDiskHttpRequest
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		public UpdateFileRequest(string uid)
		{
			Url = "/api/v1/resources/" + uid;
		}
	}
}
