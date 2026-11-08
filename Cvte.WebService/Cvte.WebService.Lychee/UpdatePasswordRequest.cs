using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Lychee
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
