using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class LoginAccountData
	{
		[DataMember(Name = "token")]
		public string Token { get; set; }

		[DataMember(Name = "user")]
		public UserDetails User { get; set; }
	}
}
