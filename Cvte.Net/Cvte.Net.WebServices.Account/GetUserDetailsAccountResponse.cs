using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class GetUserDetailsAccountResponse : AccountHttpResponse
	{
		[DataMember(Name = "data")]
		public UserDetails User { get; set; }
	}
}
