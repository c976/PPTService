using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.CloudDisk
{
	[DataContract]
	internal class GetFileDetailRequest : CloudDiskHttpRequest
	{
		
		public GetFileDetailRequest(string fileId)
		{
			Url = "/api/v1/resources/" + fileId;
		}
	}
}
