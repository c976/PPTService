using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.Http
{
	
	
	public class DownloadFileRequest : HttpRequest
	{
		public string FilePath { get; set; }

		public long Size { get; set; }

		public string Hash { get; set; }
	}
}
