using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account.Seewo.UserCenter.Models
{
	
	
	[Request("auth/checkToken")]
	[DataContract]
	public class CheckTokenRequest : JsonRequest
	{
		[DataMember(Name = "token")]
		public string Token { get; set; }

		public CheckTokenRequest(string token)
		{
			Token = token;
		}
	}
}
