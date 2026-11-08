using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class GetSubjectsBody : AccountRequestBody
	{
		[DataMember(Name = "tokenId")]
		public string TokenId { get; set; }

		[DataMember(Name = "timeStamp")]
		public string TimeSpan { get; set; }
	}
}
