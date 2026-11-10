using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;
using Cvte.WebService.Internal;

namespace Cvte.WebService.Account
{
	[Reqeust("users", ReqeustType.Post, ApiPrefix = "/api/v1/")]
	[DataContract]
	internal class RegisterRequest : AccountRequestBase<RegisterRequestBody>
	{
		public RegisterRequest(string userId, string password, string userName, string appName)
		{
			Body = new RegisterRequestBody
			{
				UserId = userId,
				UserName = userName,
				Password = Md5Hash.smethod_0(password ?? string.Empty).ToLower(),
				AppName = appName
			};
		}
	}
}
