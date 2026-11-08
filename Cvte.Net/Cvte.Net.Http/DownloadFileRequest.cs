using System.Runtime.CompilerServices;

namespace Cvte.Net.Http
{
	
	
	public class DownloadFileRequest : HttpRequest
	{
		public string FilePath { get; set; }

		public long Size { get; set; }

		public string Hash { get; set; }
	}
}
