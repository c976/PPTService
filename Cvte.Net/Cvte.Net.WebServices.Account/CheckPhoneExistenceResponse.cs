using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class CheckPhoneExistenceResponse : AccountHttpResponse
	{
		[DataMember(Name = "data")]
		public CheckUserExistenceData Data { get; set; }

		public bool IsUsed => Data?.IsUsed ?? false;
	}
}
