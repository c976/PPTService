using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class CheckValidateCodeRequestBody : AccountRequestBody
	{
		[DataMember(Name = "target")]
		public string Target { get; set; }

		[DataMember(Name = "dynamic")]
		public string Code { get; set; }

		[DataMember(Name = "isUsed")]
		public string OneTime { get; set; } = "true";
	}
}
