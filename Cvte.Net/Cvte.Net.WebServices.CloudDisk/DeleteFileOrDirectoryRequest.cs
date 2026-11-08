using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.CloudDisk
{
	[Request("", RequestMethods.Delete)]
	[DataContract]
	internal class DeleteFileOrDirectoryRequest : CloudDiskHttpRequest
	{
		
		public DeleteFileOrDirectoryRequest(string uid)
		{
			Url = "/api/v1/resources/" + uid;
		}
	}
}
