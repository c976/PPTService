using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	
	
	[Request("/api/v2/login")]
	[DataContract]
	internal class LoginAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "app")]
		public string AppName { get; set; }

		[DataMember(Name = "account")]
		public string UserName { get; set; }

		[DataMember(Name = "password")]
		public string Password { get; set; }
	}
}
