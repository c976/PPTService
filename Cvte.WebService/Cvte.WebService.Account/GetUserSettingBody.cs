using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class GetUserSettingBody : AccountRequestBody
	{
		[DataMember(Name = "resourceid")]
		public string ResourceId { get; set; }

		[DataMember(Name = "tokenId")]
		public string TokenId { get; set; }

		[DataMember(Name = "timeStamp")]
		public string TimeSpan { get; set; }
	}
}
