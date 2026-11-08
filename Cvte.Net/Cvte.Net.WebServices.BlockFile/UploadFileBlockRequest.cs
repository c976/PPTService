using System.Runtime.CompilerServices;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	public class UploadFileBlockRequest : HttpRequest
	{
		public string TaskId { get; set; }

		public string UploadId { get; set; }

		public string UploadUrl { get; set; }

		public string CompleteUrl { get; set; }
	}
}
