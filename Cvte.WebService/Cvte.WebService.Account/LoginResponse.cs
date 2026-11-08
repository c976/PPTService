using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	public class LoginResponse : AccountResponse
	{
		[DataMember(Name = "userId")]
		public string ResourceId { get; set; }

		[DataMember(Name = "tokenId")]
		public string TokenId { get; set; }

		[DataMember(Name = "userName")]
		public string UserId { get; set; }

		[DataMember(Name = "nickName")]
		public string UserName { get; set; }

		[DataMember(Name = "photoUrl")]
		public string UserPhoto { get; set; }

		[DataMember(Name = "phone")]
		public string Phone { get; set; }
	}
}
