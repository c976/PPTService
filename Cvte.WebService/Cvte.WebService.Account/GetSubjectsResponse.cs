using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class GetSubjectsResponse : AccountResponse
	{
		[DataMember(Name = "data")]
		public IList<Stage> Data { get; set; }
	}
}
