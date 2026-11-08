using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	[Request("api/v1/auth/login")]
	[DataContract]
	internal class LoginAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "username")]
		public string UserName { get; set; } = "";

		[DataMember(Name = "password")]
		public string Password { get; set; } = "";

		
		[DataMember(Name = "captcha")]
		
		public Captcha Captcha
		{
			
			get;
			
			set;
		}
	}
}
