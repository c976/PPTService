using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class SendValidateCodeRequestBody : AccountRequestBody
	{
		[DataMember(Name = "target")]
		public string Target { get; set; }

		[DataMember(Name = "sendType")]
		public string Type { get; set; }

		[DataMember(Name = "action")]
		public string Action { get; set; }
	}
}
