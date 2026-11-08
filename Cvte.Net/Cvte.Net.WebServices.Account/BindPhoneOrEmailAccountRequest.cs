using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	[Request("api/v1/auth/users/bind", RequestMethods.Put)]
	[DataContract]
	internal class BindPhoneOrEmailAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "uid")]
		public string UserId { get; set; }

		[DataMember(Name = "phone")]
		public string Phone { get; set; }

		[DataMember(Name = "dynamicCode")]
		public string ValidateCode { get; set; }
	}
}
