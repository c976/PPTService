using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[Reqeust("dynamic/text/verification", ReqeustType.Post, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class CheckValidateCodeRequest : AccountRequestBase<CheckValidateCodeRequestBody>
	{
		public CheckValidateCodeRequest(string target, string code)
		{
			Body = new CheckValidateCodeRequestBody
			{
				Target = target,
				Code = code
			};
		}
	}
}
