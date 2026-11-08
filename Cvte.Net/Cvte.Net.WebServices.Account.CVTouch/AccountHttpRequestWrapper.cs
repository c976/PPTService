using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	
	
	internal class AccountHttpRequestWrapper : FormsRequest
	{
		[DataMember(Name = "data")]
		public string Data { get; internal set; }

		[DataMember(Name = "sign")]
		public string Sign { get; internal set; }
	}
}
