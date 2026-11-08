using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	public class ParsingPptLycheeHttpResponse : LycheeHttpResponse
	{
		public PptParseResultType ResultType { get; set; }

		public IList<string> DownloadUrls { get; set; }
	}
}
