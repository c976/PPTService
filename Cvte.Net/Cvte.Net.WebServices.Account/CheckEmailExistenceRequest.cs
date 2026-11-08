using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	[Request("api/v1/auth/email", RequestMethods.Get)]
	[DataContract]
	internal class CheckEmailExistenceRequest : AccountHttpRequest
	{
		[DataMember(Name = "email")]
		public string Email { get; set; }
	}
}
