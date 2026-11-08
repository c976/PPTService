using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	[Request("api/v1/auth/stages/subjects")]
	[DataContract]
	internal class GetStageAndSubjectAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "useExtendSS")]
		public bool UseExtendSs { get; set; }

		public GetStageAndSubjectAccountRequest(bool useExtendSs)
		{
			UseExtendSs = useExtendSs;
		}
	}
}
