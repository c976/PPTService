using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Lychee
{
	[Reqeust("/cgi/api/pre/complete", ReqeustType.Post)]
	[DataContract]
	internal class CompleteRequest : LycheeRequestBase
	{
		[DataMember(Name = "key")]
		public string Key { get; set; }

		[DataMember(Name = "uid")]
		public string Uid { get; set; }

		[DataMember(Name = "pid")]
		public string Pid { get; set; }
	}
}
