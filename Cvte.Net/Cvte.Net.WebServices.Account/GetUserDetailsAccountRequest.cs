using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	[Request("api/v1/auth/users/info")]
	[DataContract]
	internal class GetUserDetailsAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "useExtendSS")]
		public bool UseExtendSs { get; set; }

		public GetUserDetailsAccountRequest(bool useExtendSs)
		{
			UseExtendSs = useExtendSs;
		}
	}
}
