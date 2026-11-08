using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	public class UploadFileInfo
	{
		public string FilePath { get; set; }

		public long FileSize { get; set; }

		public string TaskId { get; set; }

		public string UploadId { get; set; }

		public string UploadUrl { get; set; }

		public string CompleteUrl { get; set; }
	}
}
