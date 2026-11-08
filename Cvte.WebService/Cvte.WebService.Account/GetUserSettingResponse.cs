using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class GetUserSettingResponse : AccountResponse
	{
		[DataMember(Name = "data")]
		public UserSettingInfo Data { get; set; }
	}
}
