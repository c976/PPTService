using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class GetCaptchaResponse : AccountHttpResponse
	{
		[DataMember(Name = "data")]
		public Captcha Captcha { get; set; }
	}
}
