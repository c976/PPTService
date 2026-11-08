using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	[Request("api/v1/auth/dynamics")]
	[DataContract]
	internal class SendValidateCodeAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "username")]
		public string UserName { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "captcha")]
		public Captcha Captcha { get; set; }
	}
}
