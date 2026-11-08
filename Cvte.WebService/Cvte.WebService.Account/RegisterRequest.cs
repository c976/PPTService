using System.Runtime.Serialization;
using _0006;
using Cvte.Pipe.Http.Request;

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
				Password = SA_Helpers.Str(password ?? string.Empty).ToLower(),
				AppName = appName
			};
		}
	}
}
