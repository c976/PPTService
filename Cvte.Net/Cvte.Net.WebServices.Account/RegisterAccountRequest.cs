using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	[Request("api/v1/auth/users")]
	[DataContract]
	internal class RegisterAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "user")]
		public UserDetails User { get; set; }

		[DataMember(Name = "dynamicCode")]
		public string ValidateCode { get; set; }
	}
}
