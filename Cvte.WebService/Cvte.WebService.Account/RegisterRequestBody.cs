using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class RegisterRequestBody : AccountRequestBody
	{
		[DataMember(Name = "userName")]
		public string UserId { get; set; }

		[DataMember(Name = "userPassword")]
		public string Password { get; set; }

		[DataMember(Name = "cnName")]
		public string UserName { get; set; }

		[DataMember(Name = "systemCode")]
		public string AppName { get; set; }
	}
}
