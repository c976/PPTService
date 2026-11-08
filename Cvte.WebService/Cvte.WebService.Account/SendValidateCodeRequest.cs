using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("dynamic/text/sendAction", ReqeustType.Post, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class SendValidateCodeRequest : AccountRequestBase<SendValidateCodeRequestBody>
	{
		internal SendValidateCodeRequest(string target, string action)
		{
			Body = new SendValidateCodeRequestBody
			{
				Target = target,
				Action = action,
				Type = "1"
			};
		}
	}
}
