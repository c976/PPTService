using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class ResetPasswordRequestBody : AccountRequestBody
	{
		[DataMember(Name = "userName")]
		public string UserId { get; set; }

		[DataMember(Name = "dynamic")]
		public string Dynamic { get; set; }

		[DataMember(Name = "newPwd")]
		public string Password { get; set; }
	}
}
