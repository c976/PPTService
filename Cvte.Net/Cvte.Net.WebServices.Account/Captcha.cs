using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class Captcha
	{
		[DataMember(Name = "key")]
		public string Key { get; set; }

		[DataMember(Name = "captcha")]
		public string Content { get; set; }
	}
}
