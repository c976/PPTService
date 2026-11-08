using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Lychee
{
	[Reqeust("/cgi/api/pre/start", ReqeustType.Post)]
	[DataContract]
	internal class StartRequest : LycheeRequestBase
	{
		[DataMember(Name = "pid")]
		public string Pid { get; set; }
	}
}
