using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class RegisterResponse : AccountResponse
	{
		[DataMember(Name = "resourceid")]
		public string UserId { get; set; }

		[DataMember(Name = "tokenId")]
		public string TokenId { get; set; }
	}
}
