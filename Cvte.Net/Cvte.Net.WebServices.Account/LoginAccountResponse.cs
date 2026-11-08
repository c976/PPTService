using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class LoginAccountResponse : AccountHttpResponse
	{
		[DataMember(Name = "data")]
		public LoginAccountData Data { get; set; }

		public string Token => Data?.Token;

		public string UserId => Data?.User?.UserId;

		public string UserName => Data?.User?.UserName;

		public string Nickname => Data?.User?.Nickname;

		public string UserPhoto => Data?.User?.UserPhoto;

		public string Phone => Data?.User?.Phone;
	}
}
