using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Lychee
{
	[Reqeust("/cgi/api/pre/token", ReqeustType.Post)]
	[DataContract]
	internal class ApplyTokenRequest : LycheeRequestBase
	{
		[DataMember(Name = "pid")]
		public string Pid { get; set; }

		[DataMember(Name = "author")]
		public string Author { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "device")]
		public string Device { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "pagecount")]
		public int Count { get; set; }

		[DataMember(Name = "scale")]
		public double Scale { get; set; }
	}
}
