using System.Runtime.Serialization;
using PPTServiceUltra.Core.Pipe.Http.Request;

namespace PPTServiceUltra.Core.WebService.Lychee
{
	[Reqeust("/cgi/api/pre/updatePwd", ReqeustType.Post)]
	[DataContract]
	internal class UpdatePasswordRequest : LycheeRequestBase
	{
		[DataMember(Name = "uuid")]
		public string Uuid { get; set; }

		[DataMember(Name = "pid")]
		public string Pid { get; set; }

		[DataMember(Name = "passwd")]
		public string Password { get; set; }

		[DataMember(Name = "authtype")]
		public string AuthType { get; set; }
	}
}
