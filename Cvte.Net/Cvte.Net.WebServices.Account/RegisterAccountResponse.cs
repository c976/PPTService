using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class RegisterAccountResponse : AccountHttpResponse
	{
		[DataMember(Name = "data")]
		public RegisterAccountData Data { get; set; }

		public string UserId => Data?.User?.UserId;

		public string TokenId => Data?.TokenId;
	}
}
