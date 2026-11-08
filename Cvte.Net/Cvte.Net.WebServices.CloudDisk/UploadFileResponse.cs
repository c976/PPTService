using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	public class UploadFileResponse : CloudDiskHttpResponse
	{
		public string Uid { get; set; }

		public string Name { get; set; }

		public string Hash { get; set; }

		public long Size { get; set; }
	}
}
