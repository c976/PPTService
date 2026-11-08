using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class RegisterAccountData
	{
		[DataMember(Name = "user")]
		public UserDetails User { get; set; }

		[DataMember(Name = "token")]
		public string TokenId { get; set; }
	}
}
