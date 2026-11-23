using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PPTServiceUltra.Core.Net.Http;

namespace PPTServiceUltra.Core.Net.WebServices.Lychee
{
	
	
	[Request("/cgi/api/pre/token")]
	[DataContract]
	internal class ApplyTokenRequest : LycheeHttpRequest
	{
		[DataMember(Name = "pid")]
		public string Pid { get; set; }

		[DataMember(Name = "author")]
		public string Author { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "pagecount")]
		public int Count { get; set; }

		[DataMember(Name = "scale")]
		public double Scale { get; set; }
	}
}
