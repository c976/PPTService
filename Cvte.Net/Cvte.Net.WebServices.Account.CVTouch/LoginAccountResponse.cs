using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	
	
	[DataContract]
	public class LoginAccountResponse : AccountHttpResponse
	{
		[DataMember(Name = "uid")]
		public string Uid { get; set; }

		[DataMember(Name = "token")]
		public string Token { get; set; }

		[DataMember(Name = "account")]
		public string UserName { get; set; }

		[DataMember(Name = "nickname")]
		public string Nickname { get; set; }

		[DataMember(Name = "avatar")]
		public string UserPhoto { get; set; }

		[DataMember(Name = "mobile")]
		public string Phone { get; set; }
	}
}
