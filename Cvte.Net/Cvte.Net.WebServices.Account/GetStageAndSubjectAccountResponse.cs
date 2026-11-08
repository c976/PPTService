using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class GetStageAndSubjectAccountResponse : AccountHttpResponse
	{
		[DataMember(Name = "data")]
		public IList<Stage> Data { get; set; }
	}
}
