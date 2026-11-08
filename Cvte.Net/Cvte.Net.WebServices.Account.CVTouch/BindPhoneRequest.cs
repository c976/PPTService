using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	
	
	[Request("/api/v2/bind")]
	[DataContract]
	internal class BindPhoneRequest : AccountHttpRequest
	{
		[DataMember(Name = "app")]
		public string AppName { get; set; }

		[DataMember(Name = "type")]
		public int ActionType { get; set; }

		[DataMember(Name = "mobile")]
		public string Mobile { get; set; }

		[DataMember(Name = "verify")]
		public string ValidateCode { get; set; }
	}
}
