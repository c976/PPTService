using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class LoginReqeustBody : AccountRequestBody
	{
		[DataMember(Name = "timeStamp")]
		public string TimeStamp { get; set; }

		[DataMember(Name = "appName")]
		public string AppName { get; set; }

		[DataMember(Name = "userName")]
		public string UserId { get; set; }

		[DataMember(Name = "password")]
		public string Password { get; set; }

		[DataMember(Name = "expireTime")]
		public string ExpireTime { get; set; }
	}
}
