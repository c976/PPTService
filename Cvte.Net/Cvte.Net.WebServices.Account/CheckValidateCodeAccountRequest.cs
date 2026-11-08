using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	[Request("api/v1/auth/dynamics/check")]
	[DataContract]
	internal class CheckValidateCodeAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "username")]
		public string UserName { get; set; }

		[DataMember(Name = "dynamicCode")]
		public string ValidateCode { get; set; }
	}
}
