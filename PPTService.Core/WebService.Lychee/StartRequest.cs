using System.Runtime.Serialization;
using PPTService.Core.Pipe.Http.Request;

namespace PPTService.Core.WebService.Lychee
{
	[Reqeust("/cgi/api/pre/start", ReqeustType.Post)]
	[DataContract]
	internal class StartRequest : LycheeRequestBase
	{
		[DataMember(Name = "pid")]
		public string Pid { get; set; }
	}
}
