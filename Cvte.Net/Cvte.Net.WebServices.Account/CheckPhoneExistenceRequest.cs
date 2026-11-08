using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	[Request("api/v1/auth/phones", RequestMethods.Get)]
	[DataContract]
	internal class CheckPhoneExistenceRequest : AccountHttpRequest
	{
		[DataMember(Name = "phone")]
		public string Phone { get; set; }
	}
}
