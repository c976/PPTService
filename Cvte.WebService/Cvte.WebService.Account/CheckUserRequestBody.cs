using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class CheckUserRequestBody : AccountRequestBody
	{
		[DataMember(Name = "userName")]
		public string UserId { get; set; }
	}
}
